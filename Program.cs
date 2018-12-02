/*@@@@@@@@@@@@@@  MikroTik Auto Backup Application  @@@@@@@@@@@@@@@@
@@@@@@@@@@@@      Author: Grzegorz Budny                @@@@@@@@@@@@
@@@@@@@@@@@@@@@@@@    Version: 1.0.2       @@@@@@@@@@@@@@@@@@@@@@@@@
@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*/
using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Net.Mail;

namespace backup_mt
{
    class Program
    {
        public void backup(string FtpServerAddress, string FtpUserName, string FtpPassword, string FtpFilePath, string DestinationPath, string DestinationFileName, string CfgFilePath, string DestinationCfgName)
        {
        
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + FtpServerAddress);

            ftpRequest.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
            Console.WriteLine("Delete status: {0}", response.StatusDescription);

            using (WebClient ftpClient = new WebClient())
            {
                ftpClient.Credentials = new System.Net.NetworkCredential(FtpUserName, FtpPassword);

                string CfgPath = "ftp://" + FtpServerAddress + "/" + CfgFilePath;
                string Cfgtrnsfrpth = @"" + DestinationPath + "/" + DestinationCfgName;

                string path = "ftp://" + FtpServerAddress + "/" + FtpFilePath;
                string trnsfrpth = @"" + DestinationPath + "/" + DestinationFileName;

                ftpClient.DownloadFile(path, trnsfrpth);
                ftpClient.DownloadFile(CfgPath, Cfgtrnsfrpth);


            }

        }
        public void sendEmail()
        {

            DateTime date = DateTime.UtcNow.Date;
            string folderName = date.ToString("dd.MM.yyyy");

            try
            {
                
                XDocument doc = XDocument.Load("Backup_mt.exe.config");

                var smtServerDoc = doc.Descendants("smtp_server");
                var senderDoc = doc.Descendants("sender_name");
                var reciepantDoc = doc.Descendants("reciepant_name");
                var portDoc = doc.Descendants("smtp_port");
                var logPathDoc = doc.Descendants("log_path");

                string[] smtpServerAddress = XDocument.Load("Backup_mt.exe.config").Descendants("smtp_server").Select(x => x.Value).ToArray();
                string[] senderAddress = XDocument.Load("Backup_mt.exe.config").Descendants("sender_name").Select(x => x.Value).ToArray();
                string[] reciepantAddress = XDocument.Load("Backup_mt.exe.config").Descendants("reciepant_name").Select(x => x.Value).ToArray();
                string[] portNumber = XDocument.Load("Backup_mt.exe.config").Descendants("smtp_port").Select(x => x.Value).ToArray();
                string[] logPath = XDocument.Load("Backup_mt.exe.config").Descendants("log_path").Select(x => x.Value).ToArray();


                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(smtpServerAddress[0]);
                mail.From = new MailAddress(senderAddress[0]);
                mail.To.Add(reciepantAddress[0]);
                mail.Subject = "MikroTik Backup notification!";
                mail.Body = "The backup has completed. Please review the log file in the attachment for details!";

                Attachment attachment = new Attachment(logPath[0] + folderName + ".txt");
                mail.Attachments.Add(attachment);
                attachment.Name = "MT Backup " + folderName + ".txt";

                SmtpServer.Port = Convert.ToInt32(portNumber[0]);
                //SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {

            Program MtBackup = new Program();

            //Reading XML file 
            XDocument doc = XDocument.Load("Backup_mt.exe.config");
            var IPs = doc.Descendants("IP");
            var User = doc.Descendants("User");
            var Pass = doc.Descendants("Pass");
            var Name = doc.Descendants("Name");
            int countDevices = 0;

            string[] Ips = XDocument.Load("Backup_mt.exe.config").Descendants("IP").Select(x => x.Value).ToArray();
            string[] Users = XDocument.Load("Backup_mt.exe.config").Descendants("User").Select(x => x.Value).ToArray();
            string[] Passes = XDocument.Load("Backup_mt.exe.config").Descendants("Pass").Select(x => x.Value).ToArray();
            string[] Names = XDocument.Load("Backup_mt.exe.config").Descendants("Name").Select(x => x.Value).ToArray();

            //Count number of devices
            foreach (var IP in IPs)
            {

                countDevices++;

            }

            //Backup folder name 
            DateTime date = DateTime.UtcNow.Date;
            string folderName = date.ToString("dd.MM.yyyy");
            string time = DateTime.Now.ToString("HH:mm:ss tt");

            var fileNameDoc = doc.Descendants("file_name");
            var logPathDoc = doc.Descendants("log_path");
            var destPathDoc = doc.Descendants("destination_path");
            var cfgNameDoc = doc.Descendants("config_name");

            string[] destPath = XDocument.Load("Backup_mt.exe.config").Descendants("destination_path").Select(x => x.Value).ToArray();
            string[] fileName = XDocument.Load("Backup_mt.exe.config").Descendants("file_name").Select(x => x.Value).ToArray();
            string[] logPath = XDocument.Load("Backup_mt.exe.config").Descendants("log_path").Select(x => x.Value).ToArray();
            string[] cfgName = XDocument.Load("Backup_mt.exe.config").Descendants("config_name").Select(x => x.Value).ToArray();
            // string[] Names = XDocument.Load("Backup_mt.exe.config").Descendants("Name").Select(x => x.Value).ToArray();

            try
            {
                //Creates new backup directory
                DirectoryInfo di = Directory.CreateDirectory(Path.Combine(destPath[0], folderName));
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(folderName));
                //File.AppendAllText(logPath+folderName+".txt",
                //Environment.NewLine + "The directory was created successfully at {0}." +  Directory.GetCreationTime(folderName) + Environment.NewLine);
                File.AppendAllText(logPath[0] + folderName + ".txt", Environment.NewLine + "NEW BACKUP TASK HAS STARTED AT: " + time + Environment.NewLine);

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            finally { }

            for (int i = 0; i < countDevices; i++)
            {
                try
                {

                    MtBackup.backup(Ips[i], Users[i], Passes[i], fileName[0], destPath[0] + folderName, Names[i] + ".backup", cfgName[0], Names[i] + ".rsc");

                    Console.WriteLine((i + 1) + ")" + Ips[i] + ":" + Names[i] + "  Config and file backup completed!");
                    // File.WriteAllText("C:/Test/test.txt", (i + 1) + ")" + addr[i] + ":" + names[i] + "  Backup completed!");
                    File.AppendAllText(logPath[0] + folderName + ".txt",
                  (i + 1) + ")" + Ips[i] + ":" + Names[i] + " Config and file backup completed!" + Environment.NewLine);

                }

                catch (WebException e)
                {
                    Console.WriteLine("\n Oops! " + Ips[i] + ":" + Names[i] + "  " + e.ToString());
                    File.AppendAllText(logPath[0] + folderName + ".txt",
              "\n Oops! " + Ips[i] + ":" + Names[i] + "  " + e.ToString() + Environment.NewLine);

                }
            }

            MtBackup.sendEmail();

        }

    }

}

