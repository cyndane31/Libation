﻿using ApplicationServices;
using Dinah.Core;
using Dinah.Core.Threading;

namespace LibationWinForms
{
	public partial class Form1
	{
		private System.ComponentModel.BackgroundWorker updateCountsBw = new();

		protected void Configure_BackupCounts()
		{
			// init formattable
			beginBookBackupsToolStripMenuItem.Format(0);
			beginPdfBackupsToolStripMenuItem.Format(0);

			Load += setBackupCounts;
            LibraryCommands.LibrarySizeChanged += setBackupCounts;
            LibraryCommands.BookUserDefinedItemCommitted += setBackupCounts;

			updateCountsBw.DoWork += UpdateCountsBw_DoWork;
			updateCountsBw.RunWorkerCompleted += exportMenuEnable;
			updateCountsBw.RunWorkerCompleted += updateBottomStats;
			updateCountsBw.RunWorkerCompleted += update_BeginBookBackups_menuItem;
			updateCountsBw.RunWorkerCompleted += udpate_BeginPdfOnlyBackups_menuItem;
		}

		private bool runBackupCountsAgain;

		private void setBackupCounts(object _, object __)
		{
			runBackupCountsAgain = true;

			if (!updateCountsBw.IsBusy)
				updateCountsBw.RunWorkerAsync();
		}

		private void UpdateCountsBw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			while (runBackupCountsAgain)
			{
				runBackupCountsAgain = false;
				e.Result = LibraryCommands.GetCounts();
			}
		}

		private void exportMenuEnable(object _, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			var libraryStats = e.Result as LibraryCommands.LibraryStats;
			Invoke(() => exportLibraryToolStripMenuItem.Enabled = libraryStats.HasBookResults);
		}

		private void updateBottomStats(object _, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			var libraryStats = e.Result as LibraryCommands.LibraryStats;
			statusStrip1.UIThreadAsync(() => backupsCountsLbl.Text = libraryStats.StatusString);
		}

		// update 'begin book and pdf backups' menu item
		private void update_BeginBookBackups_menuItem(object _, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			var libraryStats = e.Result as LibraryCommands.LibraryStats;

			var menuItemText
				= libraryStats.HasPendingBooks
				? $"{libraryStats.PendingBooks} remaining"
				: "All books have been liberated";
			menuStrip1.UIThreadAsync(() =>
			{
				beginBookBackupsToolStripMenuItem.Format(menuItemText);
				beginBookBackupsToolStripMenuItem.Enabled = libraryStats.HasPendingBooks;
			});
		}

		// update 'begin pdf only backups' menu item
		private void udpate_BeginPdfOnlyBackups_menuItem(object _, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			var libraryStats = e.Result as LibraryCommands.LibraryStats;

			var menuItemText
				= libraryStats.pdfsNotDownloaded > 0
				? $"{libraryStats.pdfsNotDownloaded} remaining"
				: "All PDFs have been downloaded";
			menuStrip1.UIThreadAsync(() =>
			{
				beginPdfBackupsToolStripMenuItem.Format(menuItemText);
				beginPdfBackupsToolStripMenuItem.Enabled = libraryStats.pdfsNotDownloaded > 0;
			});
		}
    }
}
