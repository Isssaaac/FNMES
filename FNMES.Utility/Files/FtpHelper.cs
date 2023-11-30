using FluentFTP;
using System.IO;
using System;
using System.Net;
using System.Threading.Tasks;
using log4net.Repository.Hierarchy;
using SqlSugar;
using ServiceStack;
using FluentFTP.Helpers;

namespace FNMES.Utility.Files
{
    public class FtpHelper
    {
        private readonly string _host;
        private readonly string _username;
        private readonly string _password;

        public FtpHelper(string host, string username, string password)
        {
            _host = host;
            _username = username;
            _password = password;
        }

        public void UploadFile(string localPath, string remotePath)
        {
            using (FtpClient client = new FtpClient(_host, _username, _password))
            {
                try
                {
                    client.Connect();
                    client.UploadFile(localPath, remotePath);
                }
                catch (FtpCommandException ex)
                {
                    //Console.WriteLine("FTP Command Exception: " + ex.Message);
                    // Handle exception
                    throw ex;
                }
                catch (FtpException ex)
                {
                    //Console.WriteLine("FTP Exception: " + ex.Message);
                    // Handle exception
                    throw ex;
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

        public void DownloadFile(string localPath, string remotePath)
        {
            using (FtpClient client = new FtpClient(_host, _username, _password))
            {
                try
                {
                    client.AutoConnect();
                    client.DownloadFile(localPath, remotePath);
                }
                catch (FtpCommandException ex)
                {
                    //Console.WriteLine("FTP Command Exception: " + ex.Message);
                    // Handle exception
                    throw ex;
                }
                catch (FtpException ex)
                {
                    //Console.WriteLine("FTP Exception: " + ex.Message);
                    // Handle exception
                    throw ex;
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }
        public Stream DownloadFileStream(string remotePath)
        {
            try
            {
                using (FtpClient client = new FtpClient(_host, _username, _password))
                {
                    client.Connect();

                    if (!client.FileExists(remotePath))
                    {
                        throw new FileNotFoundException("File not found on FTP server.");
                    }
                    MemoryStream stream = new MemoryStream();
                     client.Download(stream, remotePath);
                    stream.Position = 0;
                    return stream;
                }
            }
            catch (FtpCommandException ex)
            {
                //Console.WriteLine("FTP Command Exception: " + ex.Message);
                // Handle exception

                throw ex;

            }
            catch (FtpException ex)
            {
                //Console.WriteLine("FTP Exception: " + ex.Message);
                // Handle exception
                throw ex;
            }
        }

        public async Task<bool> FtpFileExistsAsync(string filePath)
        {
            try
            {
                using (FtpClient ftpClient = new FtpClient(_host, _username, _password))
                {
                    await ftpClient.ConnectAsync();

                    if (await ftpClient.FileExistsAsync(filePath))
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void UploadServerFileToFtp(string sourceUrl, string filePath)
        {
            try
            {
                using (FtpClient client = new FtpClient(_host, _username, _password))
                {
                    try
                    {
                        client.Connect();
                        if(client.FileExists(filePath)) {
                            return;
                        }
                        string tempFileName = SnowFlakeSingle.instance.NextId().ToString() + "tempFile.dpf";
                        using (WebClient webClient = new WebClient())
                        {
                            //client.Credentials = new NetworkCredential(username, password);
                            webClient.DownloadFile(sourceUrl, tempFileName);
                        }
                        client.UploadFile(tempFileName, filePath);
                        File.Delete("tempFile.txt");
                    }
                    catch (FtpCommandException ex)
                    {
                        //Console.WriteLine("FTP Command Exception: " + ex.Message);
                        // Handle exception
                        throw ex;
                    }
                    catch (FtpException ex)
                    {
                        //Console.WriteLine("FTP Exception: " + ex.Message);
                        // Handle exception
                        throw ex;
                    }
                    finally
                    {
                        client.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }

}
