using IdeaManagement.Models;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace IdeaManagement.Services;

public class DatabaseCredentialManager
{
    private const string CredentialTarget = "IdeaManagementApp:DatabaseConnection";
    private const string CredentialComment = "Database connection credentials for Idea Management Application";

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CREDENTIAL
    {
        public int Flags;
        public int Type;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public int CredentialBlobSize;
        public IntPtr CredentialBlob;
        public int Persist;
        public int AttributeCount;
        public IntPtr Attributes;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetAlias;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string UserName;
    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredWriteW(ref CREDENTIAL credential, uint flags);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredReadW(string target, int type, int reservedFlag, out IntPtr credentialPtr);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool CredFree(IntPtr cred);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredDeleteW(string target, int type, int flags);

    public void SaveCredentials(DatabaseCredentials credentials)
    {
        var jsonCredentials = JsonSerializer.Serialize(credentials);
        var credentialBlob = Encoding.Unicode.GetBytes(jsonCredentials);

        var cred = new CREDENTIAL
        {
            Type = 1, // CRED_TYPE_GENERIC
            TargetName = CredentialTarget,
            Comment = CredentialComment,
            CredentialBlobSize = credentialBlob.Length,
            CredentialBlob = Marshal.AllocHGlobal(credentialBlob.Length),
            Persist = 2, // CRED_PERSIST_LOCAL_MACHINE
            UserName = $"{credentials.Username}@{credentials.Server}"
        };

        try
        {
            Marshal.Copy(credentialBlob, 0, cred.CredentialBlob, credentialBlob.Length);

            // Delete existing credential if it exists
            CredDeleteW(CredentialTarget, 1, 0);

            if (!CredWriteW(ref cred, 0))
            {
                throw new Exception($"Failed to save credentials. Error code: {Marshal.GetLastWin32Error()}");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(cred.CredentialBlob);
        }
    }

    public DatabaseCredentials? LoadCredentials()
    {
        IntPtr credentialPtr;
        if (!CredReadW(CredentialTarget, 1, 0, out credentialPtr))
        {
            var error = Marshal.GetLastWin32Error();
            if (error == 1168) // ERROR_NOT_FOUND
                return null;
            throw new Exception($"Failed to read credentials. Error code: {error}");
        }

        try
        {
            var credential = Marshal.PtrToStructure<CREDENTIAL>(credentialPtr);
            var blob = new byte[credential.CredentialBlobSize];
            Marshal.Copy(credential.CredentialBlob, blob, 0, credential.CredentialBlobSize);
            var json = Encoding.Unicode.GetString(blob);
            return JsonSerializer.Deserialize<DatabaseCredentials>(json);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to decode stored credentials", ex);
        }
        finally
        {
            CredFree(credentialPtr);
        }
    }

    public bool RemoveCredentials()
    {
        return CredDeleteW(CredentialTarget, 1, 0);
    }

    public bool CredentialsExist()
    {
        IntPtr credentialPtr;
        if (!CredReadW(CredentialTarget, 1, 0, out credentialPtr))
        {
            return false;
        }
        CredFree(credentialPtr);
        return true;
    }
}
