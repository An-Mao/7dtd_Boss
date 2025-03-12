using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Boss.patch;

namespace Boss {
    public class XmlWatcher {
        private readonly string _filePath;
        private FileSystemWatcher _watcher;
        private XmlDocument _xmlDocument;

        public XmlWatcher(string filePath) {
            _filePath = filePath;
            _xmlDocument = new XmlDocument();
        }

        public async Task StartWatching() {
            // 确保文件存在
            if (!File.Exists(_filePath)) {
                Console.WriteLine($"XML file not found: {_filePath}");
                return;
            }

            // 初始加载 XML 文件
            LoadXmlFile();

            // 创建 FileSystemWatcher
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(_filePath), Path.GetFileName(_filePath)) {
                NotifyFilter = NotifyFilters.LastWrite, // 仅监视 LastWrite 更改
                EnableRaisingEvents = true,
            };

            // 添加事件处理程序
            _watcher.Changed += OnFileChanged;
            _watcher.Error += OnError;

            Console.WriteLine($"Watching for changes in XML file: {_filePath}");
            await Task.Delay(-1); // 保持程序运行
        }

        public void StopWatching() {
            if (_watcher != null) {
                _watcher.EnableRaisingEvents = false;
                _watcher.Changed -= OnFileChanged;
                _watcher.Error -= OnError;
                _watcher.Dispose();
                Console.WriteLine($"Stopped watching XML file: {_filePath}");
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e) {
            if (e.ChangeType == WatcherChangeTypes.Changed) {
                Console.WriteLine($"XML file changed: {_filePath}");
                LoadXmlFile();
            }
        }

        private void OnError(object sender, ErrorEventArgs e) {
            Console.WriteLine($"Error watching XML file: {_filePath} - {e.GetException()}");
        }

        private void LoadXmlFile() {
            try {
                _xmlDocument.Load(_filePath);
                Console.WriteLine($"XML file reloaded successfully: {_filePath}");
                LoadConfigs.LoadBossConfig(_filePath);
            } catch (Exception ex) {
                Console.WriteLine($"Error loading XML file: {_filePath} - {ex}");
            }
        }


        public XmlDocument GetXmlDocument() {
            return _xmlDocument;
        }
    }
}
