
using nn.account;

public class AxiNSMount
{
	static bool bInMount = false;
	internal static string m_SaveMountName;
	static bool bInMountForDebug = false;
	internal static string m_SaveMountForDebugName;


	public bool SaveIsMount => bInMount;
	public string SaveMountName
	{
		get
		{
			if (!bInMount)
				return string.Empty;
			else
				return m_SaveMountName;
		}
	}
	public bool MountSave(Uid userId, string mountName = "save")
	{
		if (bInMount)
			return true;

		if (!nn.fs.SaveData.IsExisting(userId))
		{
			UnityEngine.Debug.LogError($"{userId.ToString()}�浵�����ڣ�");
			return false;
		}
		UnityEngine.Debug.Log($"{userId.ToString()}�浵ȷ�����ڣ�");

		nn.Result result;
		result = nn.fs.SaveData.Mount(mountName, userId);
		//result.abortUnlessSuccess();

		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"MountSave->����{mountName}:/ ʧ��: " + result.ToString());
			return false;
		}
		UnityEngine.Debug.Log($"MountSave->����{mountName}:/ �ɹ� ");
		m_SaveMountName = mountName;
		bInMount = true;
		return true;
	}


	public bool MountSDForDebug(string mountName = "sd")
	{
		if (bInMountForDebug)
			return true;
		nn.Result result;
		result = nn.fs.SdCard.MountForDebug(mountName);
		//result.abortUnlessSuccess();
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"nn_fs_MountSdCardForDebug->����{mountName}:/ ʧ��: " + result.ToString());
			return false;
		}
		UnityEngine.Debug.Log($"nn_fs_MountSdCardForDebug->����{mountName}:/ �ɹ� ");
		m_SaveMountForDebugName = mountName;
		bInMountForDebug = true;
		return true;
	}

	public bool MountSD(string mountName = "sd")
	{
		if (bInMountForDebug)
			return true;
		nn.Result result;
		result = nn.fs.SdCard.Mount(mountName);
		//result.abortUnlessSuccess();
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"nn_fs_MountSdCard->����{mountName}:/ ʧ��: " + result.ToString());
			return false;
		}
		UnityEngine.Debug.Log($"nn_fs_MountSdCard->����{mountName}:/ �ɹ� ");
		m_SaveMountForDebugName = mountName;
		bInMountForDebug = true;
		return true;
	}


	public void UnmountSave()
	{
		if (!bInMount)
		{
			UnityEngine.Debug.LogError($"{m_SaveMountName}:/ û�б����أ�����ж��");
			return;
		}
		nn.fs.FileSystem.Unmount(m_SaveMountName);
		UnityEngine.Debug.LogError($"UnmountSaveForDebufa->��ж��{m_SaveMountName}:/ ");
		bInMount = false;
	}

	public void UnmountSaveForDebug()
	{
		if (!bInMountForDebug)
		{
			UnityEngine.Debug.LogError($"{m_SaveMountForDebugName}:/ û�б����أ�����ж��");
			return;
		}
		nn.fs.FileSystem.Unmount(m_SaveMountForDebugName);
		UnityEngine.Debug.LogError($"UnmountSaveForDebufa->��ж��{m_SaveMountForDebugName}:/ ");
		bInMountForDebug = false;
	}

}
