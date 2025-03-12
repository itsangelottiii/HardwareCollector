Public Class frmMain
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SystemInfoCollector.CollectAndSaveSystemInfo(TextBox1)
    End Sub
End Class
