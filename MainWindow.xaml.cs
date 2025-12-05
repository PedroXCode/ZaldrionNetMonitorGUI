using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace ZaldrionNetMonitorGUI
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<ConnectionInfo> Connections { get; } = new();

        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            ConnectionsDataGrid.ItemsSource = Connections;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += (s, e) => RefreshConnections();
            _timer.Start();

            RefreshConnections();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshConnections();
        }

        private void ExportJsonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    FileName = $"ZaldrionNetMonitor_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (dlg.ShowDialog() == true)
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var data = Connections.ToList();
                    string json = JsonSerializer.Serialize(data, options);
                    File.WriteAllText(dlg.FileName, json);
                    MessageBox.Show("Exportación completada.", "ZaldrionNetMonitor", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "ZaldrionNetMonitor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshConnections()
        {
            try
            {
                string processFilter = ProcessFilterTextBox.Text?.Trim() ?? string.Empty;
                string protocolFilter = (ProtocolComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Todos";

                var newList = NetstatParser.GetActiveConnections();

                // Aplicar filtros
                var filtered = newList.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(processFilter))
                {
                    filtered = filtered.Where(c => c.ProcessName.IndexOf(processFilter, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (protocolFilter == "TCP" || protocolFilter == "UDP")
                {
                    filtered = filtered.Where(c => string.Equals(c.Protocol, protocolFilter, StringComparison.OrdinalIgnoreCase));
                }

                var finalList = filtered.ToList();

                // Actualizar la ObservableCollection eficientemente
                Connections.Clear();
                foreach (var c in finalList)
                {
                    Connections.Add(c);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al refrescar conexiones: {ex.Message}", "ZaldrionNetMonitor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public static class NetstatParser
    {
        public static System.Collections.Generic.List<ConnectionInfo> GetActiveConnections()
        {
            var list = new System.Collections.Generic.List<ConnectionInfo>();

            var psi = new ProcessStartInfo
            {
                FileName = "netstat",
                Arguments = "-ano",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            if (proc == null)
                return list;

            using var reader = proc.StandardOutput;
            string? line;
            bool headerSkipped = false;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Saltar encabezados hasta la línea de guiones
                if (!headerSkipped)
                {
                    if (line.StartsWith("Proto", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase) ||
                        line.StartsWith("UDP", StringComparison.OrdinalIgnoreCase))
                    {
                        headerSkipped = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (line.StartsWith("TCP") || line.StartsWith("UDP"))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 4)
                        continue;

                    string protocol = parts[0];
                    string local = parts[1];
                    string remote = parts[2];

                    string state = "-";
                    string pidStr;

                    if (protocol.Equals("TCP", StringComparison.OrdinalIgnoreCase))
                    {
                        if (parts.Length < 5)
                            continue;

                        state = parts[3];
                        pidStr = parts[4];
                    }
                    else
                    {
                        // UDP típicamente: Proto LocalAddress ForeignAddress PID
                        pidStr = parts[^1];
                    }

                    if (!int.TryParse(pidStr, out int pid))
                        pid = 0;

                    string processName = "Unknown";
                    if (pid != 0)
                    {
                        try
                        {
                            var p = Process.GetProcessById(pid);
                            processName = p.ProcessName;
                        }
                        catch
                        {
                            // ignorar
                        }
                    }

                    // Parsear IP y puerto (simple, puede fallar con IPv6 pero OK para la mayoría)
                    (string localAddr, int localPort) = SplitAddressPort(local);
                    (string remoteAddr, int remotePort) = SplitAddressPort(remote);

                    list.Add(new ConnectionInfo
                    {
                        Protocol = protocol,
                        State = state,
                        ProcessName = processName,
                        Pid = pid,
                        LocalAddress = localAddr,
                        LocalPort = localPort,
                        RemoteAddress = remoteAddr,
                        RemotePort = remotePort,
                        LastSeen = DateTime.Now.ToString("HH:mm:ss")
                    });
                }
            }

            return list;
        }

        private static (string addr, int port) SplitAddressPort(string input)
        {
            try
            {
                // Manejo simple para IPv4: "192.168.1.2:443"
                int lastColon = input.LastIndexOf(':');
                if (lastColon > 0 && lastColon < input.Length - 1)
                {
                    string addr = input[..lastColon];
                    string portStr = input[(lastColon + 1)..];
                    if (int.TryParse(portStr, out int port))
                    {
                        return (addr, port);
                    }
                }
            }
            catch
            {
                // ignorar
            }

            return (input, 0);
        }
    }
}
