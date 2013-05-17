using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeplyScriptTest
{
    /// <summary>
    /// It does some common preparation for tests.
    /// Methods are intended to read naturally like Prepare.Database() to be read "prepare database"
    /// </summary>
    static class Prepare
    {
        /// <summary>
        /// Sets machine name to an alias and returns it. 
        /// Note: Make sure C:\servername.txt does not exist before tests.
        /// </summary>
        /// <returns></returns>
        public static string Machine()
        {
            var machineName = "TestMachine";
            Environment.SetEnvironmentVariable("MACHINE_ALIAS", machineName);

            return machineName;
        }

        /// <summary>
        /// Stores an unchanged web.config in the expected path for Runner and returns the url
        /// </summary>
        /// <returns></returns>
        public static string WebConfig()
        {
            var pathWeb = Path.Combine(Environment.CurrentDirectory, "Website");
            if (!Directory.Exists(pathWeb))
            {
                Directory.CreateDirectory(pathWeb);
            }

            var webConfig = Path.Combine(pathWeb, "web.config");
            #region write web.config
            File.WriteAllText(webConfig, @"<?xml version='1.0' encoding='utf-8'?>
<configuration>
  <configSections>
    <section name='dataProviderModel' type='Application.DataModel.DataProviderModelConfigurationSection, Application.DataModel' />
    <sectionGroup name='elmah'>
      <section name='security' requirePermission='false' type='Elmah.SecuritySectionHandler, Elmah' />
      <section name='errorLog' requirePermission='false' type='Elmah.ErrorLogSectionHandler, Elmah' />
      <section name='errorMail' requirePermission='false' type='Elmah.ErrorMailSectionHandler, Elmah' />
      <section name='errorFilter' requirePermission='false' type='Elmah.ErrorFilterSectionHandler, Elmah' />
    </sectionGroup>
  </configSections>
  <connectionStrings>
    <add name='AppDatabase' providerName='System.Data.SqlClient' connectionString='Server=.\SQLEXPRESS; uid=App; password=VERY_SECURE_PASSWORD; MultipleActiveResultSets=True; Database=xyz;' />
    <add name='WiredContactDatabase' providerName='System.Data.SqlClient' connectionString='database=XYZ_2012_11_19; Server=.\SQLEXPRESS; uid=App; password=VERY_SECURE_PASSWORD;  MultipleActiveResultSets=True;' />
    <add name='DMDatabase' providerName='System.Data.SqlClient' connectionString='database=XYZ; Server=ME; uid=App; password=VERY_SECURE_PASSWORD;  MultipleActiveResultSets=True;' />
  </connectionStrings>
  <appSettings>
    <add key='UploadFolder' value='App_Documents' />
    <add key='UploadFolder.VirtualRoot' value='/App_Documents/' />
    <add key='CuteEditorDefaultFilesPath' value='~/Content/@CuteSoft/CuteEditor' />
    <add key='Email.Enable.Ssl' value='true' />
    <add key='Email.Sender.Address' value='admin@gmail.com' />
    <add key='Email.Sender.Name' value='Gmail admin' />
    <add key='Email.Permitted.Domains' value='*' />
    <add key='Email.Maximum.Retries' value='4' />
    <add key='Log.Record.Application.Events' value='false' />
    <add key='Database.Cache.Enabled' value='true' />
    <add key='Error.Notification.Receiver' value='' />
    <add key='Use.Light.Transaction' value='false' />
    <add key='Database.Session.Memory.Enabled' value='false' />
    <add key='Data.Access.Log.Custom.Queries' value='false' />
    <add key='NHibernate.DataProvider.Log.Query.Calls' value='false' />
    <add key='NHibernate.DataProvider.Profile.Queries' value='false' />
    <add key='Render.IE.Compatible.With.IE7' value='false' />
    <add key='PdfConverter.LicenseKey' value='INSERT_HERE' />
    <add key='CKeditor:BasePath' value='~/Content/@Ckeditor' />
    <add key='ChartImageHandler' value='storage=file;timeout=20;dir=c:\TempImageFiles\;' />
    <add key='NHibernate.Support.Bypassing.Validation' value='true' />
    <add key='WiredImporter.InsertOnly' value='false' />
    <add key='WiredImporter.ImportDone' value='true' />
    <add key='DMImporter.InsertOnly' value='false' />
    <add key='DMImporter.ImportDone' value='true' />
  </appSettings>
  <elmah>
    <errorLog type='Elmah.XmlFileErrorLog, Elmah' logPath='~/App_Data/Errors' />
    <security allowRemoteAccess='true' />
  </elmah>
  <system.net>
    <mailSettings>
      <smtp deliveryMethod='Network'>
        <network host='smtp.gmail.com' port='587' userName='admin@gmail.com' password='VERY_SECURE_PASSWORD' defaultCredentials='true' />
      </smtp>
    </mailSettings>
  </system.net>
  <!--Ajax stuff:-->
  <dataProviderModel>
    <providers>
      <add assembly='Project.Model' providerFactoryType='App.DAL.AdoDotNetDataProviderFactory, Project.Model' />
    </providers>
  </dataProviderModel>
  <system.web>
    <httpRuntime maxRequestLength='2147483647' requestValidationMode='2.0' enableVersionHeader='false' />
    <pages enableEventValidation='false' pageBaseType='Application.UI.Page, Application.UI' validateRequest='false'>
      <namespaces>
        <add namespace='System.Linq' />
        <add namespace='Application.Model' />
        <add namespace='Application.DataModel' />
        <add namespace='App' />
      </namespaces>
      <controls>
        <add tagPrefix='cke' namespace='CKEditor.NET' assembly='CKEditor.NET' />
        <add tagPrefix='asp' namespace='Application.UI' assembly='Application.UI' />
        <add tagPrefix='asp' namespace='Application.UI.Controls' assembly='Application.UI' />
        <add tagPrefix='asp' namespace='AjaxControlToolkit' assembly='AjaxControlToolkit' />
        <add tagPrefix='asp' namespace='System.Web.UI.DataVisualization.Charting' assembly='System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' />
      </controls>
    </pages>
    <httpHandlers>
      <add path='elmah.axd' verb='POST,GET,HEAD' type='Elmah.ErrorLogPageFactory, Elmah' />
      <add path='ChartImg.axd' verb='GET,HEAD,POST' type='System.Web.UI.DataVisualization.Charting.ChartHttpHandler, System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' validate='false' />
    </httpHandlers>
    <compilation debug='true' defaultLanguage='c#' targetFramework='4.0'>
      <assemblies>
        <add assembly='System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089' />
        <add assembly='System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A' />
        <add assembly='System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089' />
        <add assembly='System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A' />
        <add assembly='System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089' />
        <add assembly='System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A' />
        <add assembly='System.Runtime.Remoting, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089' />
        <add assembly='System.Transactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089' />
        <add assembly='System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35' />
        <add assembly='System.Data.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089' />
      </assemblies>
      <buildProviders>
        <!--<add extension='.rdlc' type='Microsoft.Reporting.RdlBuildProvider, Microsoft.ReportViewer.Common, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' />-->
        <add extension='.asbx' type='Microsoft.Web.Services.BridgeBuildProvider' />
      </buildProviders>
    </compilation>
    <customErrors mode='Off' />
    <trace enabled='false' requestLimit='10' pageOutput='false' traceMode='SortByTime' localOnly='true' />
    <sessionState mode='InProc' stateConnectionString='tcpip=127.0.0.1:42424' sqlConnectionString='data source=127.0.0.1; Trusted_Connection=yes' cookieless='false' timeout='20' />
    <globalization requestEncoding='utf-8' responseEncoding='utf-8' culture='en-GB' uiCulture='en-GB' />
    <webServices>
      <protocols>
        <add name='HttpGet' />
        <add name='HttpPost' />
      </protocols>
    </webServices>
    <authentication mode='Forms'>
      <forms loginUrl='Login.aspx' protection='All' timeout='20' slidingExpiration='true' />
    </authentication>
    <authorization>
      <allow users='*' />
    </authorization>
    <httpModules>
      <add name='ErrorLog' type='Elmah.ErrorLogModule, Elmah' />
      <add name='ErrorMail' type='Elmah.ErrorMailModule, Elmah' />
      <add name='ErrorFilter' type='Elmah.ErrorFilterModule, Elmah' />
    </httpModules>
  </system.web>
  <system.codedom>
    <compilers>
      <compiler language='c#;cs;csharp' extension='.cs' type='Microsoft.CSharp.CSharpCodeProvider,System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' warningLevel='4' compilerOptions='/warnaserror-'>
        <providerOption name='CompilerVersion' value='v4.0' />
      </compiler>
    </compilers>
  </system.codedom>
  <runtime>
    <assemblyBinding xmlns='urn:schemas-microsoft-com:asm.v1'>
      <probing privatePath='bin;Framework;' />
    </assemblyBinding>
  </runtime>
  <system.webServer>
    <handlers>
      <!--<add name='HTML Pagehandler' path='*.html' verb='*' type='System.Web.UI.PageHandlerFactory' resourceType='Unspecified' preCondition='integratedMode' />-->
      <remove name='ChartImageHandler' />
      <add name='Elmah' path='elmah.axd' verb='POST,GET,HEAD' type='Elmah.ErrorLogPageFactory, Elmah' preCondition='integratedMode' />
      <add name='ChartImageHandler' preCondition='integratedMode' verb='GET,HEAD,POST' path='ChartImg.axd' type='System.Web.UI.DataVisualization.Charting.ChartHttpHandler, System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' />
    </handlers>
    <httpProtocol>
      <customHeaders>
        <remove name='Server' />
        <remove name='X-Powered-By' />
        <remove name='X-AspNet-Version' />
      </customHeaders>
    </httpProtocol>
    <validation validateIntegratedModeConfiguration='false' />
    <modules>
      <add name='ErrorLog' type='Elmah.ErrorLogModule, Elmah' preCondition='managedHandler' />
      <add name='ErrorMail' type='Elmah.ErrorMailModule, Elmah' preCondition='managedHandler' />
      <add name='ErrorFilter' type='Elmah.ErrorFilterModule, Elmah' preCondition='managedHandler' />
    </modules>
  </system.webServer>
  <location path='elmah.axd' allowOverride='false'>
    <system.web>
      <httpHandlers>
        <add verb='POST,GET,HEAD' path='elmah.axd' type='Elmah.ErrorLogPageFactory, Elmah' />
      </httpHandlers>
    </system.web>
    <system.webServer>
      <handlers>
        <add name='ELMAH' verb='POST,GET,HEAD' path='elmah.axd' type='Elmah.ErrorLogPageFactory, Elmah' preCondition='integratedMode' />
      </handlers>
    </system.webServer>
  </location>
</configuration>");
            #endregion

            return webConfig;
        }

        /// <summary>
        /// Creates a script (usually to be included by other scripts) in the current path
        /// </summary>
        public static void Script(string fileName, string content)
        {
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, fileName), content);
        }

        /// <summary>
        /// Creates a database and fills it with test data
        /// </summary>
        public static void Database()
        {
            CreateDatabase();

            DatabaseUtil.Execute("CREATE TABLE Customers (Id uniqueidentifier NOT NULL, Name nvarchar(50) NOT NULL, Age int NULL)");
            DatabaseUtil.Execute("INSERT INTO Customers(Id, Name, Age) VALUES ('11111111-0000-0000-0000-000000000000','John Smith', 35)");
            DatabaseUtil.Execute("INSERT INTO Customers(Id, Name, Age) VALUES ('22222222-0000-0000-0000-000000000000','John Doe', 18)");
            DatabaseUtil.Execute("INSERT INTO Customers(Id, Name, Age) VALUES ('33333333-0000-0000-0000-000000000000','Laura Lindberg', 79)");
            DatabaseUtil.Execute("INSERT INTO Customers(Id, Name, Age) VALUES ('44444444-0000-0000-0000-000000000000','Daniel Eriksson', 21)");

            DeployScript.DatabaseManager.ExecuteSqlOverride = DatabaseUtil.ExecuteInSqlCe;
        }

        private static void CreateDatabase()
        {
            // prepare the folder
            if (!Directory.Exists(DatabaseUtil.DatabasePath))
            {
                Directory.CreateDirectory(DatabaseUtil.DatabasePath);
            }

            // clear existing database
            if (File.Exists(DatabaseUtil.DatabaseFile))
            {
                File.Delete(DatabaseUtil.DatabaseFile);
            }

            // and really create the file
            using (var engine = new SqlCeEngine(DatabaseUtil.ConnectionString))
            {
                engine.CreateDatabase();
            }
        }
    }
}
