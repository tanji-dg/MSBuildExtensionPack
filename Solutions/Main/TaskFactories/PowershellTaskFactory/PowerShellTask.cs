﻿//-------------------------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerShellTask.cs">(c) 2017 Mike Fourie and Contributors (https://github.com/mikefourie/MSBuildExtensionPack) under MIT License. See https://opensource.org/licenses/MIT </copyright>
// This task is based on code from (http://code.msdn.microsoft.com/PowershellFactory). It is used here with permission.
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------
namespace MSBuild.ExtensionPack.TaskFactory
{
    using System;
    using System.Management.Automation.Runspaces;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// A task that executes a Windows PowerShell script.
    /// </summary>
    internal class PowerShellTask : Task, IGeneratedTask, IDisposable
    {
        /// <summary>
        /// The context that the Windows PowerShell script will run under.
        /// </summary>
        private Pipeline pipeline;

        internal PowerShellTask(string script)
        {
            this.pipeline = RunspaceFactory.CreateRunspace().CreatePipeline();
            this.pipeline.Commands.AddScript(script);
            this.pipeline.Runspace.Open();
            this.pipeline.Runspace.SessionStateProxy.SetVariable("log", this.Log);
        }

        public object GetPropertyValue(TaskPropertyInfo property)
        {
            return this.pipeline.Runspace.SessionStateProxy.GetVariable(property.Name);
        }

        public void SetPropertyValue(TaskPropertyInfo property, object value)
        {
            this.pipeline.Runspace.SessionStateProxy.SetVariable(property.Name, value);
        }

        public override bool Execute()
        {
            this.pipeline.Invoke();
            return !this.Log.HasLoggedErrors;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.pipeline.Runspace != null)
                {
                    this.pipeline.Runspace.Dispose();
                    this.pipeline = null;
                }
            }
        }
    }
}
