namespace ZJYFrameWork.Common {
public enum Message {
/// <summary>
/// Cn
/// </summary>
LANGUAGE_CODE,
/// <summary>
/// 账号为空
/// </summary>
error_account_not_blank,
/// <summary>
/// 账号不存在
/// </summary>
error_account_not_exit,
/// <summary>
/// 账号或密码错误
/// </summary>
error_account_password,
/// <summary>
/// 协议参数错误，请联系客服，我们会尽快解决
/// </summary>
error_protocol_param,
/// <summary>
/// token
/// </summary>
error_user_orm_no_uid,
/// <summary>
/// 两次密码不一致
/// </summary>
error_account_password_not_affirm,
/// <summary>
/// 账号已存在
/// </summary>
error_account_already_exists,
/// <summary>
/// 密码长度不符合要求
/// </summary>
error_password_length,
/// <summary>
/// 密码包含空格，请重新设置
/// </summary>
error_password_not_have_null,
MAX_NUM
}
}
