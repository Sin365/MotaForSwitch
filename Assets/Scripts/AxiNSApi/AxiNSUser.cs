using nn.account;

public class AxiNSUser
{
	bool m_bInit = false;
	bool m_bGotOpenPreselectedUser = false;
	Uid m_UserId;
	nn.account.UserHandle mUserHandle;
	nn.account.Nickname m_NickName;

	#region ���⹫���ӿڣ�ȷ���ڲ���ȫ�����������
	public bool GetUserID(out Uid uid)
	{
		InitPreselectedUserInfo();
		if (!m_bGotOpenPreselectedUser)
		{
			uid = Uid.Invalid;
			return false;
		}
		uid = m_UserId;
		return true;
	}
	public bool GetNickName(out string NickName)
	{
		InitPreselectedUserInfo();
		if (!m_bGotOpenPreselectedUser)
		{
			NickName = string.Empty;
			return false;
		}
		NickName = m_NickName.ToString();
		return true;
	}
	#endregion
	/// <summary>
	/// ��ʼ��Accountģ���
	/// </summary>
	void InitNSAccount()
	{
		if (m_bInit)
			return;
		//�����ȳ�ʼ��NS��Account ��Ȼ���ü���
		nn.account.Account.Initialize();
		m_bInit = true;
	}

	/// <summary>
	/// ��ȡԤѡ�û�
	/// </summary>
	void InitPreselectedUserInfo()
	{
		if (m_bGotOpenPreselectedUser)
			return;

		InitNSAccount();
		nn.Result result;
		mUserHandle = new nn.account.UserHandle();
		if (!nn.account.Account.TryOpenPreselectedUser(ref mUserHandle))
		{
			UnityEngine.Debug.LogError("��Ԥѡ���û�ʧ��.");
			return;
		}
		UnityEngine.Debug.Log("��Ԥѡ�û��ɹ�.");
		result = nn.account.Account.GetUserId(ref m_UserId, mUserHandle);
		//result.abortUnlessSuccess();
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"GetUserIdʧ��: {result.ToString()}");
			return;
		}

		if (m_UserId == Uid.Invalid)
		{
			UnityEngine.Debug.LogError("�޷���ȡ�û� ID");
			return;
		}
		UnityEngine.Debug.Log($"��ȡ�û� ID:{m_UserId.ToString()}");

		result = nn.account.Account.GetNickname(ref m_NickName, m_UserId);
		//result.abortUnlessSuccess();
		if (!result.IsSuccess())
		{
			UnityEngine.Debug.LogError($"GetNicknameʧ��: {result.ToString()}");
			return;
		}
		UnityEngine.Debug.Log($"��ȡ�û� NickName ID:{m_NickName.ToString()}");
		m_bGotOpenPreselectedUser = true;
	}
}
