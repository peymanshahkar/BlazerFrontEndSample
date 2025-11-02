// فایل JavaScript برای مدیریت اسکن بارکد
let currentVideoStream = null;
let codeReader = null;

window.initializeBarcodeScanner = async () => {
    try {
        const { BrowserQRCodeReader, BrowserBarcodeReader } = ZXing;
        codeReader = new BrowserQRCodeReader();
        return true;
    } catch (error) {
        console.error('Error initializing barcode scanner:', error);
        return false;
    }
};

window.startBarcodeScan = async (dotNetHelper) => {
    try {
        const videoElement = document.getElementById('barcode-video');

        if (!videoElement) {
            throw new Error('Video element not found');
        }

        // دریافت لیست دوربین‌ها
        const videoInputDevices = await ZXing.BrowserCodeReader.listVideoInputDevices();

        // استفاده از دوربین پشتی (rear) در صورت موجود بودن
        const rearCamera = videoInputDevices.find(device =>
            device.label.toLowerCase().includes('back') ||
            device.label.toLowerCase().includes('rear')
        );

        const deviceId = rearCamera ? rearCamera.deviceId : undefined;

        // تنظیم callback برای زمانی که بارکد اسکن می‌شود
        codeReader.decodeFromVideoDevice(deviceId, videoElement, (result, err) => {
            if (result) {
                dotNetHelper.invokeMethodAsync('OnBarcodeScanned', result.text);
            }
        });

        return true;
    } catch (error) {
        console.error('Error starting barcode scan:', error);
        return false;
    }
};

window.stopBarcodeScan = () => {
    try {
        if (codeReader) {
            codeReader.reset();
            codeReader = null;
        }

        // توقف استریم دوربین
        if (currentVideoStream) {
            currentVideoStream.getTracks().forEach(track => track.stop());
            currentVideoStream = null;
        }

        return true;
    } catch (error) {
        console.error('Error stopping barcode scan:', error);
        return false;
    }
};

window.getCameraPermissions = async () => {
    try {
        const stream = await navigator.mediaDevices.getUserMedia({
            video: { facingMode: 'environment' }
        });
        currentVideoStream = stream;
        stream.getTracks().forEach(track => track.stop());
        return true;
    } catch (error) {
        console.error('Camera permission denied:', error);
        return false;
    }
};