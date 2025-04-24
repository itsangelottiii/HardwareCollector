Public Class frmMain
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SystemInfoCollector.CollectAndSaveSystemInfo(TextBox1)
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        frmInfo.ShowDialog()
    End Sub
End Class
