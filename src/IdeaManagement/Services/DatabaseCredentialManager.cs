using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Text.Json;
using IdeaManagement.Models;

namespace IdeaManagement.Services;

public class DatabaseCredentialManager
{
    private const string CredentialTarget = "IdeaManagementApp";

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CREDENTIAL
    {
        public int Flags;
        public int Type;
        public string TargetName;
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public int CredentialBlobSize;
        public IntPtr CredentialBlob;
        public int Persist;
        public int AttributeCount;
        public IntPtr Attributes;
        public string TargetAlias;
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
            CredentialBlobSize = credentialBlob.Length,
            CredentialBlob = Marshal.AllocHGlobal(credentialBlob.Length),
            Persist = 2, // CRED_PERSIST_LOCAL_MACHINE
            UserName = credentials.Username
        };

        try
        {
            Marshal.Copy(credentialBlob, 0, cred.CredentialBlob, credentialBlob.Length);
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
            return null;
        }

        try
        {
            var credential = Marshal.PtrToStructure<CREDENTIAL>(credentialPtr);
            var blob = new byte[credential.CredentialBlobSize];
            Marshal.Copy(credential.CredentialBlob, blob, 0, credential.CredentialBlobSize);
            var json = Encoding.Unicode.GetString(blob);
            return JsonSerializer.Deserialize<DatabaseCredentials>(json);
        }
        catch
        {
            return null;
        }
        finally
        {
            CredFree(credentialPtr);
        }
    }

    public void RemoveCredentials()
    {
        CredDeleteW(CredentialTarget, 1, 0);
    }
}
