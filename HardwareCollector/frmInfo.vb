Public Class frmInfo
    Private Sub frmInfo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If SystemInfoCollector.IsRemoteSession() Then
            CheckBox1.Checked = True
        Else
            CheckBox1.Checked = False
        End If
        Label1.Text = "v." & My.Application.Info.Version.ToString(2)
        Label2.Text = Environment.MachineName
    End Sub
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim myProcess = New Process()
        myProcess.StartInfo.UseShellExecute = True
        myProcess.StartInfo.FileName = "https://github.com/itsangelottiii"
        myProcess.Start()
    End Sub
End Class