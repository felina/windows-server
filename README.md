Windows Job Server
==============

Configuration and sample executables for the Windows computer vision server.

*IMPORTANT* You will need to update Web.Release.config with your AWS keys in order to properly deploy the project. To avoid accidentally uploading these changes to the git repository, you should use the following command: `git update-index --assume-unchanged JobServer/JobServer/Web.Release.config`

Contributing
--------------

To contribute, you should use Visual Studio Express 2013 for Web. Other versions should work, but this is the simplest version.
Steps to setup the repo are as follows:

1. Download and install Visual Studio 2013 for Web.
2. Install the AWS SDK for .NET
3. Install the JSON.Net package by going to (Tools > Library Package Mangager > Package Manager Console) and run 'Install-Package Newtonsoft.Json -Version 6.0.1'
4. Open the 'Team Explorer' panel (VIEW > Team Explorer)
5. In the Team Explorer panel, find 'Local Git Repositories' and press 'Clone'.
6. Enter the https URL of the repo and the path to save to, and hit 'Clone'.
7. You should now see 'windows-server' in your list of Local Git Repositories, double click it.
8. Double click 'JobServer.sln' from the list of solutions. This contains all server code.
9. After having edited the code, you can use the Team Explorer panel to both commit and push/pull.
