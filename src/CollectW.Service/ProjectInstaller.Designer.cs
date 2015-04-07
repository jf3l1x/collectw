namespace CollectW.Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CollectWInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.CollectWServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // CollectWInstaller
            // 
            this.CollectWInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.CollectWInstaller.Password = null;
            this.CollectWInstaller.Username = null;
            // 
            // CollectWServiceInstaller
            // 
            this.CollectWServiceInstaller.Description = "Performance counter collector and forwarder";
            this.CollectWServiceInstaller.DisplayName = "CollectW";
            this.CollectWServiceInstaller.ServiceName = "CollectW";
            this.CollectWServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.CollectWInstaller,
            this.CollectWServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller CollectWInstaller;
        private System.ServiceProcess.ServiceInstaller CollectWServiceInstaller;
    }
}