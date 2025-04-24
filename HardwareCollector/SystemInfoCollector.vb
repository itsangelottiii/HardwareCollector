Imports System.Xml
Imports System.Management
Imports System.IO
Imports System.Security.Cryptography
Imports System.Security.Cryptography.Xml
Imports System.Runtime.InteropServices

Public Class SystemInfoCollector
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function EnumDisplaySettings(ByVal deviceName As String, ByVal modeNum As Integer, ByRef devMode As DEVMODE) As Boolean
    End Function
    Private Shared Function GetSystemMetrics(nIndex As Integer) As Integer
    End Function
    Private Const SM_REMOTESESSION As Integer = &H1000 ' 4096

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Public Structure DEVMODE
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public dmDeviceName As String
        Public dmSpecVersion As Short
        Public dmDriverVersion As Short
        Public dmSize As Short
        Public dmDriverExtra As Short
        Public dmFields As Integer
        Public dmPositionX As Integer
        Public dmPositionY As Integer
        Public dmDisplayOrientation As Integer
        Public dmDisplayFixedOutput As Integer
        Public dmColor As Short
        Public dmDuplex As Short
        Public dmYResolution As Short
        Public dmTTOption As Short
        Public dmCollate As Short
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public dmFormName As String
        Public dmLogPixels As Short
        Public dmBitsPerPel As Integer
        Public dmPelsWidth As Integer
        Public dmPelsHeight As Integer
        Public dmDisplayFlags As Integer
        Public dmDisplayFrequency As Integer
        Public dmICMMethod As Integer
        Public dmICMIntent As Integer
        Public dmMediaType As Integer
        Public dmDitherType As Integer
        Public dmReserved1 As Integer
        Public dmReserved2 As Integer
        Public dmPanningWidth As Integer
        Public dmPanningHeight As Integer
    End Structure

    Public Shared Sub CollectAndSaveSystemInfo(textBox As TextBox)
        Try
            AppendLog(textBox, "Sammle Systeminformationen...")
            Dim doc As New XmlDocument()
            Dim root As XmlElement = doc.CreateElement("SystemInfo")
            doc.AppendChild(root)

            Dim userInfo As XmlElement = doc.CreateElement("UserInfo")
            userInfo.SetAttribute("User", Environment.UserName)
            userInfo.SetAttribute("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            root.AppendChild(userInfo)

            AppendLog(textBox, "Sammle CPU-Informationen...")
            Dim cpuElement As XmlElement = doc.CreateElement("CPU")
            Dim cpuSearcher As New ManagementObjectSearcher("SELECT Name, NumberOfCores, MaxClockSpeed FROM Win32_Processor")
            For Each obj As ManagementObject In cpuSearcher.Get()
                Dim processor As XmlElement = doc.CreateElement("Processor")
                processor.SetAttribute("Name", obj("Name").ToString())
                processor.SetAttribute("Cores", obj("NumberOfCores").ToString())
                processor.SetAttribute("MaxClockSpeedMHz", obj("MaxClockSpeed").ToString())
                cpuElement.AppendChild(processor)
            Next
            root.AppendChild(cpuElement)

            AppendLog(textBox, "Sammle RAM-Informationen...")
            Dim ramElement As XmlElement = doc.CreateElement("RAM")
            Dim ramSearcher As New ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory")
            Dim totalMemory As ULong = 0
            Dim slots As Integer = 0
            For Each obj As ManagementObject In ramSearcher.Get()
                totalMemory += Convert.ToUInt64(obj("Capacity"))
                slots += 1
            Next
            ramElement.SetAttribute("TotalMemoryGB", (totalMemory / (1024 ^ 3)).ToString("F2"))
            ramElement.SetAttribute("SlotsUsed", slots.ToString())
            root.AppendChild(ramElement)

            AppendLog(textBox, "Sammle Festplatten-Informationen...")
            Dim disksElement As XmlElement = doc.CreateElement("Disks")
            Dim diskSearcher As New ManagementObjectSearcher("SELECT DeviceID, Size, FreeSpace, FileSystem FROM Win32_LogicalDisk WHERE DriveType=3")
            For Each obj As ManagementObject In diskSearcher.Get()
                Dim disk As XmlElement = doc.CreateElement("Disk")
                disk.SetAttribute("Name", obj("DeviceID").ToString())
                disk.SetAttribute("FileSystem", obj("FileSystem").ToString())
                disk.SetAttribute("TotalSizeGB", (Convert.ToUInt64(obj("Size")) / (1024 ^ 3)).ToString("F2"))
                disk.SetAttribute("FreeSpaceGB", (Convert.ToUInt64(obj("FreeSpace")) / (1024 ^ 3)).ToString("F2"))
                disksElement.AppendChild(disk)
            Next
            root.AppendChild(disksElement)

            AppendLog(textBox, "Sammle Betriebssystem-Informationen...")
            Dim osElement As XmlElement = doc.CreateElement("OperatingSystem")
            Dim osSearcher As New ManagementObjectSearcher("SELECT Caption, Version, Locale FROM Win32_OperatingSystem")
            For Each obj As ManagementObject In osSearcher.Get()
                osElement.SetAttribute("OSFullName", obj("Caption").ToString())
                osElement.SetAttribute("OSVersion", obj("Version").ToString())
                osElement.SetAttribute("Language", obj("Locale").ToString())
            Next
            root.AppendChild(osElement)

            AppendLog(textBox, "Sammle Hardware-Informationen...")
            Dim hardwareElement As XmlElement = doc.CreateElement("Hardware")
            Dim csSearcher As New ManagementObjectSearcher("SELECT Manufacturer, Model, Name FROM Win32_ComputerSystem")
            For Each obj As ManagementObject In csSearcher.Get()
                hardwareElement.SetAttribute("Manufacturer", If(obj("Manufacturer") IsNot Nothing, obj("Manufacturer").ToString(), "N/A"))
                hardwareElement.SetAttribute("Model", If(obj("Model") IsNot Nothing, obj("Model").ToString(), "N/A"))
                hardwareElement.SetAttribute("SystemName", If(obj("Name") IsNot Nothing, obj("Name").ToString(), "N/A"))
            Next
            If IsRemoteSession() Then
                hardwareElement.SetAttribute("SessionType", "TS/RDP")
            Else
                hardwareElement.SetAttribute("SessionType", "Local")
            End If
            root.AppendChild(hardwareElement)

            AppendLog(textBox, "Sammle BIOS-Informationen...")
            Dim biosElement As XmlElement = doc.CreateElement("BIOS")
            Dim biosSearcher As New ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS")
            For Each obj As ManagementObject In biosSearcher.Get()
                biosElement.SetAttribute("SerialNumber", If(obj("SerialNumber") IsNot Nothing, obj("SerialNumber").ToString(), "N/A"))
            Next
            root.AppendChild(biosElement)

            AppendLog(textBox, "Sammle lokale Benutzer...")
            Dim localUsersElement As XmlElement = doc.CreateElement("LocalUsers")
            Dim usersSearcher As New ManagementObjectSearcher("SELECT Name, Status FROM Win32_UserAccount WHERE LocalAccount=True")
            For Each obj As ManagementObject In usersSearcher.Get()
                Dim userElement As XmlElement = doc.CreateElement("User")
                userElement.SetAttribute("Name", obj("Name").ToString())
                userElement.SetAttribute("Status", If(obj("Status") IsNot Nothing, obj("Status").ToString(), "N/A"))
                localUsersElement.AppendChild(userElement)
            Next
            root.AppendChild(localUsersElement)

            AppendLog(textBox, "Sammle Drucker-Informationen...")
            Dim printersElement As XmlElement = doc.CreateElement("Printers")
            Dim printerSearcher As New ManagementObjectSearcher("SELECT Name, DriverName, PortName FROM Win32_Printer")
            For Each obj As ManagementObject In printerSearcher.Get()
                Dim printer As XmlElement = doc.CreateElement("Printer")
                printer.SetAttribute("Name", obj("Name").ToString())
                Dim driverName As String = If(obj("DriverName") IsNot Nothing, obj("DriverName").ToString(), "N/A")
                printer.SetAttribute("Driver", driverName)
                Dim portName As String = If(obj("PortName") IsNot Nothing, obj("PortName").ToString(), "N/A")
                Dim connectionType As String = "Unbekannt"
                If portName.ToUpper().Contains("USB") Then
                    connectionType = "USB"
                ElseIf portName.ToUpper().Contains("LPT") Then
                    connectionType = "Parallel"
                ElseIf portName.ToUpper().Contains("COM") Then
                    connectionType = "Serial"
                ElseIf portName.ToUpper().Contains("IP") Or portName.ToUpper().Contains("TCP") Or portName.ToUpper().Contains("NE") Then
                    connectionType = "Network"
                Else
                    connectionType = portName
                End If
                printer.SetAttribute("ConnectionType", connectionType)
                printersElement.AppendChild(printer)
            Next
            root.AppendChild(printersElement)

            AppendLog(textBox, "Sammle Netzwerkdomäne...")
            Dim domainElement As XmlElement = doc.CreateElement("NetworkDomain")
            Dim domainSearcher As New ManagementObjectSearcher("SELECT Domain, PartOfDomain FROM Win32_ComputerSystem")
            For Each obj As ManagementObject In domainSearcher.Get()
                Dim domainName As String = obj("Domain").ToString()
                Dim partOfDomain As Boolean = Convert.ToBoolean(obj("PartOfDomain"))
                domainElement.SetAttribute("Name", domainName)
                domainElement.SetAttribute("Type", If(partOfDomain, "Domäne", "Arbeitsgruppe"))
            Next
            root.AppendChild(domainElement)

            AppendLog(textBox, "Sammle BitLocker-Status...")
            Dim bitLockerElement As XmlElement = doc.CreateElement("BitLockerStatus")
            Try
                Dim bitLockerSearcher As New ManagementObjectSearcher("SELECT ProtectionStatus FROM Win32_EncryptableVolume")
                For Each obj As ManagementObject In bitLockerSearcher.Get()
                    bitLockerElement.SetAttribute("ProtectionStatus", obj("ProtectionStatus").ToString())
                Next
            Catch ex As ManagementException
                AppendLog(textBox, "BitLocker-Informationen nicht verfügbar.")
                bitLockerElement.SetAttribute("ProtectionStatus", "Nicht verfügbar")
            End Try
            root.AppendChild(bitLockerElement)

            AppendLog(textBox, "Sammle Batterie-Informationen...")
            Dim batteryElement As XmlElement = doc.CreateElement("Battery")
            Dim batterySearcher As New ManagementObjectSearcher("SELECT EstimatedChargeRemaining, BatteryStatus, EstimatedRunTime FROM Win32_Battery")
            For Each obj As ManagementObject In batterySearcher.Get()
                batteryElement.SetAttribute("ChargeRemaining", If(obj("EstimatedChargeRemaining") IsNot Nothing, obj("EstimatedChargeRemaining").ToString(), "N/A"))
                batteryElement.SetAttribute("BatteryStatus", If(obj("BatteryStatus") IsNot Nothing, obj("BatteryStatus").ToString(), "N/A"))
                batteryElement.SetAttribute("EstimatedRunTime", If(obj("EstimatedRunTime") IsNot Nothing, obj("EstimatedRunTime").ToString(), "N/A"))
            Next
            root.AppendChild(batteryElement)

            AppendLog(textBox, "Sammle Grafikkarten-Informationen...")
            Dim gpuElement As XmlElement = doc.CreateElement("Graphics")
            Dim gpuSearcher As New ManagementObjectSearcher("SELECT Name, AdapterRAM FROM Win32_VideoController")
            For Each obj As ManagementObject In gpuSearcher.Get()
                Dim gpu As XmlElement = doc.CreateElement("GPU")
                gpu.SetAttribute("Name", obj("Name").ToString())
                gpu.SetAttribute("MemoryMB", (Convert.ToUInt64(obj("AdapterRAM")) / (1024 ^ 2)).ToString("F2"))
                gpuElement.AppendChild(gpu)
            Next
            root.AppendChild(gpuElement)

            AppendLog(textBox, "Sammle Monitor-Informationen...")
            Dim monitorsElement As XmlElement = doc.CreateElement("Monitors")
            For Each scr As Screen In Screen.AllScreens
                Dim monitor As XmlElement = doc.CreateElement("Monitor")
                monitor.SetAttribute("DeviceName", scr.DeviceName)
                monitor.SetAttribute("Resolution", scr.Bounds.Width.ToString() & "x" & scr.Bounds.Height.ToString())
                Dim devMode As New DEVMODE()
                devMode.dmSize = CShort(Marshal.SizeOf(GetType(DEVMODE)))
                If EnumDisplaySettings(scr.DeviceName, -1, devMode) Then
                    monitor.SetAttribute("RefreshRateHz", devMode.dmDisplayFrequency.ToString())
                Else
                    monitor.SetAttribute("RefreshRateHz", "N/A")
                End If
                monitorsElement.AppendChild(monitor)
            Next
            root.AppendChild(monitorsElement)

            Dim timestamp As String = DateTime.Now.ToString("yyyyMMdd-HHmmss")
            Dim systemName As String = Environment.MachineName
            Dim xmlFileName As String = $"{timestamp}_{systemName}.xml"
            Dim xmlPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFileName)
            doc.Save(xmlPath)
            AppendLog(textBox, $"XML-Datei gespeichert: {xmlPath}")

            SignXmlFile(xmlPath)
            AppendLog(textBox, "XML-Datei wurde signiert.")

            Dim verificationResult As Boolean = VerifyXmlSignature(xmlPath, GetFixedRSAKey())
            AppendLog(textBox, $"Signaturverifikation: {(If(verificationResult, "Erfolgreich", "Fehlgeschlagen"))}")
        Catch ex As Exception
            AppendLog(textBox, $"Fehler: {ex.Message}")
        End Try
    End Sub

    Private Shared Sub SignXmlFile(xmlFilePath As String)
        Dim rsa As RSA = GetFixedRSAKey()

        Dim xmlDoc As New XmlDocument()
        xmlDoc.PreserveWhitespace = True
        xmlDoc.Load(xmlFilePath)

        Dim signedXml As New SignedXml(xmlDoc)
        signedXml.SigningKey = rsa

        Dim reference As New Reference()
        reference.Uri = ""
        reference.AddTransform(New XmlDsigEnvelopedSignatureTransform())
        signedXml.AddReference(reference)

        signedXml.ComputeSignature()
        Dim xmlDigitalSignature As XmlElement = signedXml.GetXml()
        xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, True))

        xmlDoc.Save(xmlFilePath)
    End Sub

    Public Shared Function VerifyXmlSignature(xmlFilePath As String, rsaKey As RSA) As Boolean
        Dim xmlDoc As New XmlDocument()
        xmlDoc.PreserveWhitespace = True
        xmlDoc.Load(xmlFilePath)

        Dim nodeList As XmlNodeList = xmlDoc.GetElementsByTagName("Signature")
        If nodeList.Count = 0 Then
            Throw New CryptographicException("Keine Signatur im Dokument gefunden.")
        End If

        Dim signedXml As New SignedXml(xmlDoc)
        signedXml.LoadXml(DirectCast(nodeList(0), XmlElement))
        Return signedXml.CheckSignature(rsaKey)
    End Function
    Private Shared Function GetFixedRSAKey() As RSA
        Dim rsa As RSA = RSA.Create()
        Dim rsaKeyXml As String = "<RSAKeyValue>" & vbCrLf &
            "  <Modulus>ynGM02KJ5r1e4+WFLS+rQibLkysQdXvX4w35Fz5I72RgxqBRr2/vYS1YxRABtvYdT5BZeLNumUU8EV0qNmu6cXBOgLiuEryYhg8R110ZcMJnN5qWwD9p36tPOPCRMu+6ZmcRp+zYBAEy+twR5mbxRZjAQKYrF91EBbNqzn0wenjDTpvpXenRE/W/p0FFQG16l3BMeYPcCCPJwJk72g6E5G7sTQB5WZyq6gcAZRtAeauMzhZgT2EunlvktOceprtv5Y3n/U7wbSeMlIUe2Nz2GDm47zFlrotzZeNJRaOd3PWA1P/FzNJrHXveSU7I0KpJZI6QIAUDkR+IvwXbokMX4Q==</Modulus>" & vbCrLf &
            "  <Exponent>AQAB</Exponent>" & vbCrLf &
            "  <P>02E1LRy0grRKFNTNJZH9mRuqnTOMmF9Y4CG9Xthg3R2fWn/OdjRs+d9Hn0Vy6j7B46yYk1o1rlj22hjoxJ6D/fVTOMo2MJ2k8vgdyR7WF3JAOClESXVCk7GOH7YkPdOOswqiHZI0ZX8g4/GK5M3NoncN9m6z2zFph2KwrYLnGmM=</P>" & vbCrLf &
            "  <Q>9S1wcj+vUgkXeNFM17QzJczVZaIeBRGCb9tam5lhWiUWaDM46zpQ9cGTJg0r4ol4p2TipwSCJihYARQCAE+VgsqFRgvL1ImGxB/GSftpa7LI0teuMcQheEWiZxdzlp9PYaVdvQI9lOLqPboDAFZaLkGV0L6m5bMOyUkKzbupVes=</Q>" & vbCrLf &
            "  <DP>bccqSseBp+/iGXd7I2vyfLW1ofbHqcq66YwcQUm25J8hx2pg21rWzd573+e9ipIVOBzHEJ/lCSsC6LnPCW4ZtqbWFcRO6Gi25cieD3mITpzzZTdpU40minXIDggWQlXA4pM/JoRr2dpwRnGHZNTNWvbhtX5AwjGjUMCcaUB2rWk=</DP>" & vbCrLf &
            "  <DQ>7RbeySx6jpsKHqRPabvxRhl5Nsn5u6798sstKJC/CpwYufTmktoW1kA12rMb//ouObmeUCJU9S9IQuqlMMXE7/CFxoG7vzCqqNeNOZesjyAZgYJeCfP+5b542LZd6YbvDZ8LMa6L/IEeQUiyhE388QQtKupinHd10gIXN3HEw7c=</DQ>" & vbCrLf &
            "  <InverseQ>LHAfWMusF2+Ig8zU6v55xu7Hyi6EwzKPrwZuN0kk7rPFJtraoNw2B73i9RlZYHepGC47yGmkamofczabwWaIip8YXVnR9Z3LfWDun5k2Z6D++lL/jb/A6VZUqsuST7EPDKOxyAewCKX4zPk4B4wL4KTQnwWIRkg7pkIzzwoESCw=</InverseQ>" & vbCrLf &
            "  <D>pLL4xUkXtBdAydwDGx37EJiLarHQdCRLIThO/K/5mnlpmm73xeklMUQ5Qimhm35jnkVr5Rzlz+BZ3C6VlyzRxFSAKY0WYbv5wqcj9riRaAFhBAXKu6Fbu1Kd408kcfJDDeiwnpO7spZfKWQ9U9tQUzTQxEfsK0AihRjQvilvdVKy8dWno9Gj17Q9cl7I2jbq4oou/xRRLbcTCRFxay8YwYfo5pGW+cg3SnKnpISHg0bTCOSYW1EMNTJUHMQbInDsJRuP7x9PEeKubvR+n5ds25kxB/E7whBKv+7C8/Ny69MecoOalIUxce1oJle1YrXLAuLClVobYGey4U7TWIkSeQ==</D>" & vbCrLf &
            "</RSAKeyValue>"
        rsa.FromXmlString(rsaKeyXml)
        Return rsa
    End Function
    Private Shared Sub AppendLog(textBox As TextBox, message As String)
        Dim logMessage As String = DateTime.Now.ToString("HH:mm:ss") & " - " & message & Environment.NewLine
        If textBox IsNot Nothing Then
            textBox.AppendText(logMessage)
        End If
    End Sub
    Public Shared Function IsRemoteSession() As Boolean
        Return GetSystemMetrics(SM_REMOTESESSION) <> 0
    End Function
End Class
