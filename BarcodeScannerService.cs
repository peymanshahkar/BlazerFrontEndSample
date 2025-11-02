namespace Client
{
    public class BarcodeScannerService
    {
        private List<string> _scanHistory = new List<string>();

        public event Func<string, Task>? OnBarcodeScanned;

        public async Task NotifyBarcodeScanned(string barcode)
        {
            _scanHistory.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {barcode}");

            if (OnBarcodeScanned != null)
            {
                await OnBarcodeScanned.Invoke(barcode);
            }
        }

        public List<string> GetScanHistory()
        {
            return _scanHistory;
        }

        public void ClearHistory()
        {
            _scanHistory.Clear();
        }
    }
}
