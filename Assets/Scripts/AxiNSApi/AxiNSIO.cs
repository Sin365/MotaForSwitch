#if UNITY_SWITCH
using nn.fs;
using System.Security.Cryptography;

#endif

public class AxiNSIO
{
    string save_name => AxiNS.instance.mount.SaveMountName;
    public string save_path => $"{save_name}:/";
#if UNITY_SWITCH
	private FileHandle fileHandle = new nn.fs.FileHandle();
#endif
    /// <summary>
    /// ���Path�Ƿ����
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool CheckPathExists(string filePath)
    {
#if !UNITY_SWITCH
        return false;
#else
        nn.fs.EntryType entryType = 0;
		nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);
		//result.abortUnlessSuccess();
		//����쳣������ı�Ť
		return nn.fs.FileSystem.ResultPathAlreadyExists.Includes(result);
#endif
    }
    /// <summary>
    /// ���Path�Ƿ񲻴���
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool CheckPathNotFound(string filePath)
    {
#if !UNITY_SWITCH
        return false;
#else
        nn.fs.EntryType entryType = 0;
		nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);
		//����쳣������ı�Ť
		return nn.fs.FileSystem.ResultPathNotFound.Includes(result);
#endif
    }
    /// <summary>
    /// ����Ŀ¼��Ŀ¼����Ҳ�᷵��true
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool CreateDir(string filePath)
    {
#if !UNITY_SWITCH
        return false;
#else
        // ʹ�÷�װ�������ʹ�����Ŀ¼
        if (!EnsureParentDirectory(filePath, true))
        {
            UnityEngine.Debug.LogError($"�޷�ȷ����Ŀ¼���ļ�д��ȡ��: {filePath}");
            return false;
        }
		return true;
#endif
    }
    /// <summary>
    /// ���沢�����ļ������Ŀ¼�����ڻ����Զ�����Ŀ¼��
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="bw"></param>
    /// <returns></returns>
    public bool FileToSaveWithCreate(string filePath, System.IO.MemoryStream ms)
    {
        return FileToSaveWithCreate(filePath, ms.ToArray());
    }
    /// <summary>
    /// ���沢�����ļ������Ŀ¼�����ڻ����Զ�����Ŀ¼��
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool FileToSaveWithCreate(string filePath, byte[] data)
    {
#if !UNITY_SWITCH
        return false;
#else
		if (!AxiNS.instance.mount.SaveIsMount)
		{
			UnityEngine.Debug.LogError($"Save ��δ���أ��޷��洢 {filePath}");
			return false;
		}

		nn.Result result;
#if UNITY_SWITCH && !UNITY_EDITOR
        // ��ֹ�û��ڱ���ʱ���˳���Ϸ
        // Switch ���� 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif
		// ʹ�÷�װ�������ʹ�����Ŀ¼
		if (!EnsureParentDirectory(filePath, true))
		{
			UnityEngine.Debug.LogError($"�޷�ȷ����Ŀ¼���ļ�д��ȡ��: {filePath}");
			return false;
		}

		//string directoryPath = System.IO.Path.GetDirectoryName(filePath.Replace(save_path, ""));
		//string fullDirectoryPath = $"{save_path}{directoryPath}";
		//UnityEngine.Debug.Log($"��鸸Ŀ¼: {fullDirectoryPath}");

		//nn.fs.EntryType entryType = 0;
		//result = nn.fs.FileSystem.GetEntryType(ref entryType, fullDirectoryPath);
		//if (!result.IsSuccess() && nn.fs.FileSystem.ResultPathNotFound.Includes(result))
		//{
		//	UnityEngine.Debug.Log($"��Ŀ¼ {fullDirectoryPath} �����ڣ����Դ��� (�ж����� result=>{result.ToString()})");
		//	result = nn.fs.Directory.Create(fullDirectoryPath);
		//	if (!result.IsSuccess())
		//	{
		//		UnityEngine.Debug.LogError($"������Ŀ¼ʧ��: {result.GetErrorInfo()}");
		//		return false;
		//	}
		//	UnityEngine.Debug.Log($"��Ŀ¼ {fullDirectoryPath} �����ɹ�");
		//}
		//else if (result.IsSuccess() && entryType != nn.fs.EntryType.Directory)
		//{
		//	UnityEngine.Debug.LogError($"·�� {fullDirectoryPath} �Ѵ��ڣ�������Ŀ¼");
		//	return false;
		//}
		//else if (!result.IsSuccess())
		//{
		//	UnityEngine.Debug.LogError($"��鸸Ŀ¼ʧ��: {result.GetErrorInfo()}");
		//	return false;
		//}

		if (CheckPathNotFound(filePath))
		{
			UnityEngine.Debug.Log($"�ļ�({filePath})��������Ҫ����");
			result = nn.fs.File.Create(filePath, data.Length); //this makes a file the size of your save journal. You may want to make a file smaller than this.
															   //result.abortUnlessSuccess();
			if (!result.IsSuccess())
			{
				UnityEngine.Debug.LogError($"�����ļ�ʧ�� {filePath} : " + result.GetErrorInfo());
				return false;
			}
		}
		else
			UnityEngine.Debug.Log($"�ļ�({filePath})���ڣ����ش���");

		result = File.Open(ref fileHandle, filePath, OpenFileMode.Write);
		//result.abortUnlessSuccess();
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"ʧ�� File.Open(ref filehandle, {filePath}, OpenFileMode.Write): " + result.GetErrorInfo());
			return false;
		}
		UnityEngine.Debug.Log($"�ɹ� File.Open(ref filehandle, {filePath}, OpenFileMode.Write)");

		//nn.fs.WriteOption.Flush Ӧ�þ��Ǹ���д��
		result = nn.fs.File.Write(fileHandle, 0, data, data.Length, nn.fs.WriteOption.Flush); // Writes and flushes the write at the same time
																							  //result.abortUnlessSuccess();
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError("д���ļ�ʧ��: " + result.GetErrorInfo());
			return false;
		}
		UnityEngine.Debug.Log("д���ļ��ɹ�: " + filePath);

		nn.fs.File.Close(fileHandle);

		//������ύ������û����ʵд��
		result = FileSystem.Commit(save_name);
		//result.abortUnlessSuccess();
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"FileSystem.Commit({save_name}) ʧ��: " + result.GetErrorInfo());
			return false;
		}
		UnityEngine.Debug.Log($"FileSystem.Commit({save_name}) �ɹ�: ");


#if UNITY_SWITCH && !UNITY_EDITOR
        // ֹͣ��ֹ�û��˳���Ϸ
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif

		return true;
#endif
    }
    public bool LoadSwitchDataFile(string filename, ref System.IO.MemoryStream ms)
    {
        if (LoadSwitchDataFile(filename, out byte[] outputData))
        {
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(ms))
            {
                writer.Write(outputData);
            }
            return true;
        }
        return false;
    }
    public bool LoadSwitchDataFile(string filename, out byte[] outputData)
    {
#if !UNITY_SWITCH
        outputData = null;
        return false;
#else
		outputData = null;
		if (!AxiNS.instance.mount.SaveIsMount)
		{
			UnityEngine.Debug.LogError($"Save ��δ���أ��޷���ȡ {filename}");
			return false;
		}
		if (CheckPathNotFound(filename))
			return false;

		nn.Result result;
		result = nn.fs.File.Open(ref fileHandle, filename, nn.fs.OpenFileMode.Read);
		if (result.IsSuccess() == false)
		{
			UnityEngine.Debug.LogError($"nn.fs.File.Open ʧ�� {filename} : result=>{result.GetErrorInfo()}");
			return false;   // Could not open file. This can be used to detect if this is the first time a user has launched your game. 
							// (However, be sure you are not getting this error due to your file being locked by another process, etc.)
		}
		UnityEngine.Debug.Log($"nn.fs.File.Open �ɹ� {filename}");
		long iFileSize = 0;
		result = nn.fs.File.GetSize(ref iFileSize, fileHandle);
		if (result.IsSuccess() == false)
		{
			UnityEngine.Debug.LogError($"nn.fs.File.GetSize ʧ�� {filename} : result=>{result.GetErrorInfo()}");
			return false;
		}
		UnityEngine.Debug.Log($"nn.fs.File.GetSize �ɹ� {filename},size=>{iFileSize}");

		byte[] loadedData = new byte[iFileSize];
		result = nn.fs.File.Read(fileHandle, 0, loadedData, iFileSize);
		if (result.IsSuccess() == false)
		{
			UnityEngine.Debug.LogError($"nn.fs.File.Read ʧ�� {filename} : result=>{result.GetErrorInfo()}");
			return false;
		}
		UnityEngine.Debug.Log($"nn.fs.File.Read �ɹ� {filename}");

		nn.fs.File.Close(fileHandle);

		//for (int i = 0; i < loadedData.Length; i++)
		//{
		//	UnityEngine.Debug.Log($"data[{i}]:{loadedData[i]}");
		//}

		outputData = loadedData;
		return true;
#endif
    }
    public bool DeletePathFile(string filename)
    {
#if !UNITY_SWITCH
        return false;
#else


#if UNITY_SWITCH && !UNITY_EDITOR
        // This next line prevents the user from quitting the game while saving. 
        // This is required for Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

		if (CheckPathNotFound(filename))
			return false;
		nn.Result result;
		result = nn.fs.File.Delete(filename);
		if (result.IsSuccess() == false)
		{
			UnityEngine.Debug.LogError($"nn.fs.File.Delete ʧ�� {filename} : result=>{result.GetErrorInfo()}");
			return false;   
		}
		result = nn.fs.FileSystem.Commit(save_name);
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"FileSystem.Commit({save_name}) ʧ��: " + result.GetErrorInfo());
			return false;
		}
		return true;

#if UNITY_SWITCH && !UNITY_EDITOR
        // End preventing the user from quitting the game while saving.
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif

#endif
	}
	public bool DeletePathDir(string filename)
    {
#if !UNITY_SWITCH
        return false;
#else

#if UNITY_SWITCH && !UNITY_EDITOR
        // This next line prevents the user from quitting the game while saving. 
        // This is required for Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

		if (CheckPathNotFound(filename))
			return false;
		nn.Result result;
		result = nn.fs.Directory.Delete(filename);
		if (result.IsSuccess() == false)
		{
			UnityEngine.Debug.LogError($"nn.fs.File.Delete ʧ�� {filename} : result=>{result.GetErrorInfo()}");
			return false;
		}
		result = nn.fs.FileSystem.Commit(save_name);
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"FileSystem.Commit({save_name}) ʧ��: " + result.GetErrorInfo());
			return false;
		}
		return true;

#if UNITY_SWITCH && !UNITY_EDITOR
        // End preventing the user from quitting the game while saving.
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
#endif
	}

	/// <summary>
	/// �ݹ�ɾ��Ŀ¼
	/// </summary>
	/// <param name="filename"></param>
	/// <returns></returns>
	public bool DeleteRecursivelyPathDir(string filename)
	{
#if !UNITY_SWITCH
        return false;
#else

#if UNITY_SWITCH && !UNITY_EDITOR
        // This next line prevents the user from quitting the game while saving. 
        // This is required for Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

		if (CheckPathNotFound(filename))
			return false;
		nn.Result result;
		result = nn.fs.Directory.DeleteRecursively(filename);
		if (result.IsSuccess() == false)
		{
			UnityEngine.Debug.LogError($"nn.fs.File.DeleteRecursively ʧ�� {filename} : result=>{result.GetErrorInfo()}");
			return false;
		}
		result = nn.fs.FileSystem.Commit(save_name);
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"FileSystem.Commit({save_name}) ʧ��: " + result.GetErrorInfo());
			return false;
		}
		return true;

#if UNITY_SWITCH && !UNITY_EDITOR
        // End preventing the user from quitting the game while saving.
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
#endif
	}

	/// <summary>
	/// �ݹ�ɾ�����
	/// </summary>
	/// <param name="filename"></param>
	/// <returns></returns>
	public bool CleanRecursivelyPathDir(string filename)
	{
#if !UNITY_SWITCH
        return false;
#else

#if UNITY_SWITCH && !UNITY_EDITOR
        // This next line prevents the user from quitting the game while saving. 
        // This is required for Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

		if (CheckPathNotFound(filename))
			return false;
		nn.Result result;
		result = nn.fs.Directory.CleanRecursively(filename);
		if (result.IsSuccess() == false)
		{
			UnityEngine.Debug.LogError($"nn.fs.File.DeleteRecursively ʧ�� {filename} : result=>{result.GetErrorInfo()}");
			return false;
		}
		result = nn.fs.FileSystem.Commit(save_name);
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"FileSystem.Commit({save_name}) ʧ��: " + result.GetErrorInfo());
			return false;
		}
		return true;

#if UNITY_SWITCH && !UNITY_EDITOR
        // End preventing the user from quitting the game while saving.
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
#endif
	}


	public bool RenameDir(string oldpath,string newpath)
	{
#if !UNITY_SWITCH
        return false;
#else

#if UNITY_SWITCH && !UNITY_EDITOR
        // This next line prevents the user from quitting the game while saving. 
        // This is required for Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

		if (CheckPathNotFound(oldpath))
			return false;

		nn.Result result;
		result = nn.fs.Directory.Rename(oldpath, newpath);
		if (result.IsSuccess() == false)
		{
			UnityEngine.Debug.LogError($"nn.fs.File.Rename ʧ�� {oldpath} to {newpath} : result=>{result.GetErrorInfo()}");
			return false;
		}
		result = nn.fs.FileSystem.Commit(save_name);
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"FileSystem.Commit({save_name}) ʧ��: " + result.GetErrorInfo());
			return false;
		}
		return true;

#if UNITY_SWITCH && !UNITY_EDITOR
        // End preventing the user from quitting the game while saving.
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
#endif
	}
	bool EnsureParentDirectory(string filePath, bool bAutoCreateDir = true)
    {
#if !UNITY_SWITCH
        return false;
#else
		// ����У��
		if (string.IsNullOrEmpty(filePath))
		{
			UnityEngine.Debug.LogError($"��Ч������filePath={filePath}");
			return false;
		}

		// ��ȡ·��ǰ׺���� save:/��sd:/��
		int prefixEndIndex = filePath.IndexOf(":/");
		if (prefixEndIndex == -1)
		{
			UnityEngine.Debug.LogError($"�ļ�·�� {filePath} ��ʽ��Ч��δ�ҵ� ':/' ǰ׺");
			return false;
		}
		string pathPrefix = filePath.Substring(0, prefixEndIndex + 2); // ��ȡǰ׺������ "save:/"
		string relativePath = filePath.Substring(prefixEndIndex + 2); // �Ƴ�ǰ׺���õ����·��

		// ������״̬
		if (!IsMountPointAccessible(pathPrefix))
		{
			UnityEngine.Debug.LogError($"���ص� {pathPrefix} δ���أ��޷�����·�� {filePath}");
			return false;
		}

		// ��ȡ��Ŀ¼·��
		string directoryPath = System.IO.Path.GetDirectoryName(relativePath); // ��ȡ��Ŀ¼���·��
		if (string.IsNullOrEmpty(directoryPath))
		{
			UnityEngine.Debug.Log($"�ļ�·�� {filePath} ���贴����Ŀ¼��λ�ڸ�Ŀ¼��");
			return true; // ��Ŀ¼���贴��
		}

		string fullDirectoryPath = $"{pathPrefix}{directoryPath}"; // ƴ��������Ŀ¼·��
		UnityEngine.Debug.Log($"��鸸Ŀ¼: {fullDirectoryPath}");

		// ���·���Ƿ���ڼ�������
		nn.fs.EntryType entryType = 0;
		nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, fullDirectoryPath);
		if (!result.IsSuccess() && nn.fs.FileSystem.ResultPathNotFound.Includes(result))
		{
			if (bAutoCreateDir)
			{
				// ·�������ڣ����Դ���
				UnityEngine.Debug.Log($"��Ŀ¼ {fullDirectoryPath} �����ڣ����Դ��� (�ж����� result=>{result.ToString()})");
				result = nn.fs.Directory.Create(fullDirectoryPath);
				if (!result.IsSuccess())
				{
					UnityEngine.Debug.LogError($"������Ŀ¼ʧ��: {result.GetErrorInfo()}");
					return false;
				}
				UnityEngine.Debug.Log($"��Ŀ¼ {fullDirectoryPath} �����ɹ�");
				return true;
			}
			return false;
		}
		else if (result.IsSuccess() && entryType != nn.fs.EntryType.Directory)
		{
			// ·�����ڣ�������Ŀ¼
			UnityEngine.Debug.LogError($"·�� {fullDirectoryPath} �Ѵ��ڣ�������Ŀ¼");
			return false;
		}
		else if (!result.IsSuccess())
		{
			// ��������
			UnityEngine.Debug.LogError($"��鸸Ŀ¼ʧ��: {result.GetErrorInfo()}");
			return false;
		}
		// ·����������Ŀ¼
		UnityEngine.Debug.Log($"��Ŀ¼ {fullDirectoryPath} �Ѵ�������Ч");
		return true;
		
#endif
    }
    /// <summary>
    /// ���ָ�����ص��Ƿ�ɷ���
    /// </summary>
    /// <param name="pathPrefix">·��ǰ׺������ "save:/" �� "sd:/"</param>
    /// <returns>���ص��Ƿ�ɷ���</returns>
    bool IsMountPointAccessible(string pathPrefix)
    {
#if !UNITY_SWITCH
        return false;
#else
		if (string.IsNullOrEmpty(pathPrefix))
		{
			UnityEngine.Debug.LogError($"��Ч���ص�: {pathPrefix}");
			return false;
		}

		// ����ǰ׺�жϹ��ص����Ͳ�������״̬
		if (pathPrefix == $"{save_name}:/")
		{
			if (!AxiNS.instance.mount.SaveIsMount)
			{
				UnityEngine.Debug.LogError($"{save_name}:/ δ����");
				return false;
			}
			return true;
		}
		else if (pathPrefix == "sd:/")
		{
			long freeSpace = 0;
			// ��� SD ������״̬��ʾ���������ʵ��ʵ�ֵ�����
			nn.Result result = nn.fs.FileSystem.GetFreeSpaceSize(ref freeSpace, "sd:/");
			if (!result.IsSuccess())
			{
				UnityEngine.Debug.LogError($"sd:/ δ���ػ��޷�����: {result.GetErrorInfo()}");
				return false;
			}
			return true;
		}
		else
		{
			UnityEngine.Debug.LogWarning($"δ֪���ص� {pathPrefix}���ٶ��ѹ���");
			return true; // �������ص������ʵ������ʵ��
		}
#endif
    }
}
