<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInfo
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmInfo))
        Label1 = New Label()
        CheckBox1 = New CheckBox()
        GroupBox1 = New GroupBox()
        Label2 = New Label()
        Label3 = New Label()
        Label4 = New Label()
        LinkLabel1 = New LinkLabel()
        GroupBox1.SuspendLayout()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Label1.Font = New Font("Segoe UI", 16.125F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label1.Location = New Point(300, 96)
        Label1.Margin = New Padding(2, 0, 2, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(117, 30)
        Label1.TabIndex = 0
        Label1.Text = "v."
        Label1.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' CheckBox1
        ' 
        CheckBox1.AutoCheck = False
        CheckBox1.AutoSize = True
        CheckBox1.ForeColor = Color.Black
        CheckBox1.Location = New Point(11, 36)
        CheckBox1.Margin = New Padding(2, 1, 2, 1)
        CheckBox1.Name = "CheckBox1"
        CheckBox1.Size = New Size(174, 19)
        CheckBox1.TabIndex = 1
        CheckBox1.Text = "Terminalserver/RDP-Sitzung"
        CheckBox1.UseVisualStyleBackColor = True
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(Label2)
        GroupBox1.Controls.Add(CheckBox1)
        GroupBox1.ForeColor = Color.Gray
        GroupBox1.Location = New Point(6, 6)
        GroupBox1.Margin = New Padding(2, 1, 2, 1)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Padding = New Padding(2, 1, 2, 1)
        GroupBox1.Size = New Size(410, 60)
        GroupBox1.TabIndex = 2
        GroupBox1.TabStop = False
        GroupBox1.Text = "Ihr Gerät"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.ForeColor = Color.Black
        Label2.Location = New Point(8, 19)
        Label2.Margin = New Padding(2, 0, 2, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(104, 15)
        Label2.TabIndex = 2
        Label2.Text = "COMPUTERNAME"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label3.Location = New Point(6, 75)
        Label3.Margin = New Padding(2, 0, 2, 0)
        Label3.Name = "Label3"
        Label3.Size = New Size(72, 15)
        Label3.TabIndex = 3
        Label3.Text = "Entwicklung"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(6, 90)
        Label4.Margin = New Padding(2, 0, 2, 0)
        Label4.Name = "Label4"
        Label4.Size = New Size(104, 15)
        Label4.TabIndex = 4
        Label4.Text = "Angelos Bamichas"
        ' 
        ' LinkLabel1
        ' 
        LinkLabel1.AutoSize = True
        LinkLabel1.Location = New Point(6, 105)
        LinkLabel1.Margin = New Padding(2, 0, 2, 0)
        LinkLabel1.Name = "LinkLabel1"
        LinkLabel1.Size = New Size(45, 15)
        LinkLabel1.TabIndex = 5
        LinkLabel1.TabStop = True
        LinkLabel1.Text = "GitHub"
        ' 
        ' frmInfo
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(423, 130)
        Controls.Add(LinkLabel1)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(GroupBox1)
        Controls.Add(Label1)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(2, 1, 2, 1)
        MaximizeBox = False
        MinimizeBox = False
        Name = "frmInfo"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "HardwareCollector"
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents LinkLabel1 As LinkLabel
End Class
