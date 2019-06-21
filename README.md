# Backup_mt
Backup_mt is a simple C# application that automates RouterOS *.backup* and *.rsc* files download to centralized location based 
on XML file. Additional feautre is SMTP support as well log file generation for mentioned operations. 

![](https://img.shields.io/badge/coding-c%23-informational.svg)
![](https://img.shields.io/badge/system-windows%20-blue.svg)
![](https://img.shields.io/badge/build-passing-success.svg?logo=appveyor)
![](https://img.shields.io/badge/usage-routeros-orange.svg)

## Change log 

- 1/18/2019 v1.0.3 first release
    - .*backup* and *.rsc* files download to centralized location 
    - folders creation for daily files download to ensure historical view
    - log files creation for daily files download to ensure historical view
    - devices and backup parameters stored in *.xml* file
    - SMTP support on any tcp port with annonymous sender 
   
## Prerequisites

-  RouterOS v6.37 or higher 
-  Operating System
    - [x] Windows 7 or higher
    - [x] Windows Server 2012 or higher 
    - [ ] Linux
-  Visual Studio 2012 or higher 

## Installation 

### Clone

- Clone project from repository ` https://github.com/gbudny93/Backup_mt `

### Build 

- Open project solution file with Visual Studio
- Build solution
- Go to Release location in project directory, `backup_mt.exe` and `App.config` are ready for deployment
- Create folders structure e.g 
    - Backup_mt
      - App
        - App.config
        - backup_mt.exe
      - Backup
        - Backup and config file will stored inside, automatically downloaded with today date as name
      - Logs
        - Log files will be stored with today date as name 

## Deployment

### Configuration file 

Configuration file contains all data that will be used during app run. Fill keys values according to the comments inside the file or according to example below.

```
<!--Devices section under backup files download-->  
    <Devices>
<!--Single device under backup files donwload parameters-->
      <device>
        <!--MikroTik Device IP-->
        <IP>1.1.1.2</IP>
        <!--Username alloweed to download backup files-->
        <User>mikrotik_user</User>
        <!--User password allowed to download backup files-->
        <Pass>password</Pass>
        <!--Backup files names/Device Name/Device Identifier-->
        <Name>MikroTik_Device</Name>
      </device>
    </Devices>

<!--MikroTik Backup Application Additional Backup Settings-->
    
    <Settings>
      <setting>
        <!--*.backup file name available in RouterOS file explorer-->
        <file_name>mikrotik.backup</file_name>
        <!--Destination path for backup files-->
        <destination_path>C:\Backups</destination_path>
        <!--Path for log file-->
        <log_path>C:\Logs</log_path>
        <!--*.rsc file name available in RouterOS file explorer-->
        <config_name>mikrotik.rsc</config_name>
        <!--SMTP Server IP-->
        <smtp_server>1.1.1.3</smtp_server>
        <!--SMTP port-->
        <smtp_port>25</smtp_port>
        <!--E-mail Sender name-->
        <sender_name>mikrotik@example.com</sender_name>
        <!--Reciepants name. To add more than one separate with ','-->
        <reciepant_name>recipient@example.com</reciepant_name>
      </setting>
    </Settings>
```

### MikroTik Configuration 

Despite of device type and its network configuration you need to have specified User dedicated for backup files download as well scheduled mentioned files creation in RouterOS. Below script does mentioned operations.

```
/system scheduler
add interval=10h name=backup on-event="system backup save name=today.backup" \
    policy=ftp,reboot,read,write,policy,test,password,sniff,sensitive,romon \
    start-date=jan/01/1970 start-time=00:00:00
add interval=10h name=config_backup on-event="export file=config.rsc" policy=\
    ftp,reboot,read,write,policy,test,password,sniff,sensitive,romon \
    start-date=jan/01/1970 start-time=09:37:33
/system backup save name=today.backup
/export file=config.rsc
/user group
add name=ftp policy="ftp,read,sensitive,!local,!telnet,!ssh,!reboot,!write,!policy,!test,!winbox,!password,!web,!sniff,!api,!romon,!dude"
/user 
add address=0.0.0.0/0 comment="FTP backup" disabled=no group=ftp name="ftp"
/user 
set [find name="ftp"] password="password"
```

### Windows Task Scheduler 

To run app automatically create a new task in Windows Task Scheduler with desired trigger. Important is to run app in the same location where XML file is placed.

## Built with 

- Visual Studio Community 2017 

## Authors

- Grzegorz Budny 

## License

This project is licensed under the MIT License - see the LICENSE.md file for details
