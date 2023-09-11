using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Net.Mime;
using System.IO;
using System.Text;
using FNMES.Utility.Extension;
using FNMES.Utility.Files;
using FNMES.Utility.Core;

namespace FNMES.Utility.Mail
{
    /// <summary>
    /// 邮件操作类
    /// </summary>
    public class MailUtils
    {
        public string ToMail { get; set; } // 收件人地址

        public MailUtils SetToMail(string strToMail)
        {
            this.ToMail = strToMail;
            return this;
        }


        public string SmtpHost { get; set; } // smtp服务器

        public MailUtils SetSmtpHost(string strSmtpHost)
        {
            this.SmtpHost = strSmtpHost;
            return this;
        }

        public MailUtils SetPort(int port)
        {
            this.Port = port;
            return this;
        }

        public int Port { set; get; } = 25;  //端口
        public string SendMailAddr { get; set; } // 发件人地址
        public MailUtils SetSendMailAddr(string strSendMailAddr)
        {
            this.SendMailAddr = strSendMailAddr;
            return this;
        }

        public string Password { get; set; } // 密码
        public MailUtils SetPassword(string strPassword)
        {
            this.Password = strPassword;
            return this;
        }

        public List<string> CcList { get; set; } // 抄送地址
        public MailUtils AddCC(string strCCMail)
        {
            if (this.CcList == null)
                this.CcList = new List<string>();
            CcList.Add(strCCMail);
            return this;
        }

        public List<string> BccList { get; set; }
        public MailUtils AddBCC(string strBCCMail)
        {
            if (this.BccList == null)
                this.BccList = new List<string>();
            BccList.Add(strBCCMail);
            return this;
        }

        public List<string> AttrList { get; set; } // 附件地址
        public MailUtils AddAttr(string strFilePath)
        {
            if (this.AttrList == null)
                this.AttrList = new List<string>();
            AttrList.Add(strFilePath);
            return this;
        }

        public string Subject { get; set; } // 邮件标题
        public MailUtils SetSubject(string strSubject)
        {
            this.Subject = strSubject;
            return this;
        }
        public string Content { get; set; }// 正文内容
        public bool IsHtml { get; set; } = false;// 是否使用html;

        public MailUtils SetTextContent(string content)
        {
            this.Content = content;
            IsHtml = false;
            return this;
        }
        public MailUtils SetHtmlContent(string content)
        {
            this.Content = content;
            IsHtml = true;
            return this;
        }

        public bool IsAuth { get; set; } = true;//是否验证

        public MailUtils SetAuth(bool isAuth)
        {
            IsAuth = isAuth;
            return this;
        }

        public string SendUserNickName { get; set; }// 发件人昵称
        public MailUtils SetSendNickName(string nickName)
        {
            SendUserNickName = nickName;
            return this;
        }
        public bool IsSSL { get; set; } = false;

        public MailUtils SetSSL(bool isSSL)
        {
            this.IsSSL = isSSL;
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="toMail"></param>
        /// <param name="ccList"></param>
        /// <param name="bccList"></param>
        /// <param name="attrList"></param>
        /// <param name="content"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public bool SendMail(string subject, string toMail, List<string> ccList, List<string> bccList, List<string> attrList, string content, bool isHtml)
        {
            //this.SetSendMailAddr(ConstUtils.SendMail)
            //     .SetPassword(ConstUtils.SendMailPassword)
            //     .SetSmtpHost(ConstUtils.SmtpHost)
            //     .SetSendNickName(ConstUtils.NickName)
            //     .SetPort(ConstUtils.MailPort)
            //     .SetSSL(ConstUtils.IsSSL)
            //     .SetAuth(false);
            this.SetSubject(subject)
                  .SetToMail(toMail);
            if (bccList != null)
            {
                foreach (string bccMail in bccList)
                {
                    this.AddBCC(bccMail);
                }
            }
            if (ccList != null)
            {
                foreach (string ccMail in ccList)
                {
                    this.AddCC(ccMail);
                }
            }
            if (attrList != null)
            {
                foreach (string strFilePath in attrList)
                {
                    this.AddAttr(strFilePath);
                }
            }
            if (isHtml)
            {
                this.SetHtmlContent(content);
            }
            else
            {
                this.SetTextContent(content);
            }
            return this.SendMail();
        }

        public bool SendMail()
        {
            //基础验证
            if (SendMailAddr.IsNullOrEmpty())
            {
                return false;
            }
            if (!ToMail.IsEmail())
            {
                return false;
            }
            if (SmtpHost.IsNullOrEmpty())
            {
                return false;
            }
            MailMessage message = new MailMessage();
            SmtpClient mSmtpClient = new SmtpClient();
            mSmtpClient.Host = SmtpHost;
            mSmtpClient.Port = Port;
            mSmtpClient.UseDefaultCredentials = false;
            mSmtpClient.EnableSsl = IsSSL;

            if (IsAuth)
            {
                NetworkCredential nc = new NetworkCredential(SendMailAddr, Password);
                mSmtpClient.Credentials = nc.GetCredential(mSmtpClient.Host, mSmtpClient.Port, "NTLM");
            }
            else
            {
                mSmtpClient.Credentials = new NetworkCredential(SendMailAddr, Password);
            }
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.None;
            mSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                //// 加载发件人地址
                if (!SendUserNickName.IsNullOrEmpty())
                {
                    MailAddress from = new MailAddress(SendMailAddr, SendUserNickName);
                    message.From = from;
                    message.Sender = from;
                }
                else
                {
                    MailAddress from = new MailAddress(SendMailAddr);
                    message.From = from;
                    message.Sender = from;
                }

                // 加载收件人地址
                message.To.Add(new MailAddress(ToMail));

                // 加载标题
                message.Subject = Subject;
                message.SubjectEncoding = Encoding.UTF8;
                // 设置发送时间
                //message.setSentDate(new Date());

                // 设置抄送
                if (null != CcList)
                {
                    foreach (String ccMail in CcList)
                    {
                        if (ccMail.IsEmail())
                        {
                            message.CC.Add(new MailAddress(ccMail));
                        }
                    }
                }
                // 设置抄送
                if (null != BccList)
                {
                    foreach (String bccMail in BccList)
                    {
                        if (bccMail.IsEmail())
                        {
                            message.Bcc.Add(new MailAddress(bccMail));
                        }
                    }
                }
                //添加正文内容，判断是否为html格式

                message.Body = Content;
                message.IsBodyHtml = IsHtml;
                message.BodyEncoding = Encoding.UTF8;
                message.Priority = MailPriority.Normal;

                // 添加附件
                if (null != AttrList)
                {
                    ContentDisposition disposition;
                    Attachment data;
                    foreach (String filePath in AttrList)
                    {
                        if (!File.Exists(filePath))
                        {
                            continue;
                        }
                        data = new Attachment(filePath, MediaTypeNames.Application.Octet);
                        disposition = data.ContentDisposition;
                        disposition.CreationDate = File.GetCreationTime(filePath);
                        disposition.ModificationDate = File.GetLastWriteTime(filePath);
                        disposition.ReadDate = File.GetLastAccessTime(filePath);
                        message.Attachments.Add(data);
                    }
                }
                // 把邮件发送出去
                mSmtpClient.Send(message);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
