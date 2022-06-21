﻿using System;
using System.Linq;
using Dinah.Core;
using LibationFileManager;

namespace LibationWinForms.Dialogs
{
	public partial class SettingsDialog
	{
		private void folderTemplateBtn_Click(object sender, EventArgs e) => editTemplate(Templates.Folder, folderTemplateTb);
		private void fileTemplateBtn_Click(object sender, EventArgs e) => editTemplate(Templates.File, fileTemplateTb);
		private void chapterFileTemplateBtn_Click(object sender, EventArgs e) => editTemplate(Templates.ChapterFile, chapterFileTemplateTb);

		private void Load_DownloadDecrypt(Configuration config)
		{
			inProgressDescLbl.Text = desc(nameof(config.InProgress));
			badBookGb.Text = desc(nameof(config.BadBook));
			badBookAskRb.Text = Configuration.BadBookAction.Ask.GetDescription();
			badBookAbortRb.Text = Configuration.BadBookAction.Abort.GetDescription();
			badBookRetryRb.Text = Configuration.BadBookAction.Retry.GetDescription();
			badBookIgnoreRb.Text = Configuration.BadBookAction.Ignore.GetDescription();

			inProgressSelectControl.SetDirectoryItems(new()
			{
				Configuration.KnownDirectories.WinTemp,
				Configuration.KnownDirectories.UserProfile,
				Configuration.KnownDirectories.AppDir,
				Configuration.KnownDirectories.MyDocs,
				Configuration.KnownDirectories.LibationFiles
			}, Configuration.KnownDirectories.WinTemp);
			inProgressSelectControl.SelectDirectory(config.InProgress);

			var rb = config.BadBook switch
			{
				Configuration.BadBookAction.Ask => this.badBookAskRb,
				Configuration.BadBookAction.Abort => this.badBookAbortRb,
				Configuration.BadBookAction.Retry => this.badBookRetryRb,
				Configuration.BadBookAction.Ignore => this.badBookIgnoreRb,
				_ => this.badBookAskRb
			};
			rb.Checked = true;

			folderTemplateLbl.Text = desc(nameof(config.FolderTemplate));
			fileTemplateLbl.Text = desc(nameof(config.FileTemplate));
			chapterFileTemplateLbl.Text = desc(nameof(config.ChapterFileTemplate));
			folderTemplateTb.Text = config.FolderTemplate;
			fileTemplateTb.Text = config.FileTemplate;
			chapterFileTemplateTb.Text = config.ChapterFileTemplate;
		}

		private void Save_DownloadDecrypt(Configuration config)
		{
			config.InProgress = inProgressSelectControl.SelectedDirectory;

			config.BadBook
				= badBookAskRb.Checked ? Configuration.BadBookAction.Ask
				: badBookAbortRb.Checked ? Configuration.BadBookAction.Abort
				: badBookRetryRb.Checked ? Configuration.BadBookAction.Retry
				: badBookIgnoreRb.Checked ? Configuration.BadBookAction.Ignore
				: Configuration.BadBookAction.Ask;

			config.FolderTemplate = folderTemplateTb.Text;
			config.FileTemplate = fileTemplateTb.Text;
			config.ChapterFileTemplate = chapterFileTemplateTb.Text;
		}
	}
}