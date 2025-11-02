let quaggaInitialized = false;
let currentConfig = {};
let quaggaLoaded = false;
let currentCameraId = "";
let isScanning = false;

// تابع برای بارگذاری Quagga به صورت سنتی
function loadQuagga() {
    return new Promise((resolve, reject) => {
        if (quaggaLoaded) {
            resolve();
            return;
        }

        if (typeof Quagga !== 'undefined') {
            quaggaLoaded = true;
            resolve();
            return;
        }

        const script = document.createElement('script');
        script.src = './js/quagga.min.js';
        script.onload = () => {
            setTimeout(() => {
                if (typeof Quagga !== 'undefined') {
                    quaggaLoaded = true;
                    console.log('Quagga loaded successfully');
                    resolve();
                } else {
                    reject(new Error('Quagga not loaded after script load'));
                }
            }, 100);
        };
        script.onerror = () => {
            reject(new Error('Failed to load Quagga script'));
        };

        document.head.appendChild(script);
    });
}

export async function getAvailableCameras() {
    try {
        if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
            console.warn('Media devices not supported');
            return [];
        }

        // ابتدا دسترسی به دوربین بگیریم
        const stream = await navigator.mediaDevices.getUserMedia({ video: true });
        const devices = await navigator.mediaDevices.enumerateDevices();
        const videoDevices = devices.filter(device => device.kind === 'videoinput');

        // متوقف کردن stream
        stream.getTracks().forEach(track => track.stop());

        const cameras = videoDevices.map(device => ({
            DeviceId: device.deviceId,
            Label: device.label || `دوربین ${videoDevices.indexOf(device) + 1}`,
            Kind: device.kind
        }));

        console.log(`Found ${cameras.length} cameras`);
        return cameras;
    } catch (error) {
        console.error('Error getting camera list:', error);
        return [];
    }
}

export async function startQuagga(dotNetHelper, decoder, elementId, width, height, patchSize, halfSample, numOfWorkers, frequency, cameraDeviceId) {
    try {
        if (!quaggaLoaded) {
            await loadQuagga();
        }

        if (isScanning) {
            await stopQuagga();
        }

        currentCameraId = cameraDeviceId || "";

        const constraints = {
            width: { ideal: width },
            height: { ideal: height },
            aspectRatio: { min: 1, max: 2 }
        };

        if (currentCameraId) {
            constraints.deviceId = { exact: currentCameraId };
        } else {
            constraints.facingMode = "environment";
        }

        currentConfig = {
            inputStream: {
                name: "Live",
                type: "LiveStream",
                target: document.querySelector(`#${elementId}`),
                constraints: constraints
            },
            locator: {
                patchSize: patchSize,
                halfSample: halfSample
            },
            numOfWorkers: Math.max(0, numOfWorkers),
            frequency: Math.max(1, frequency),
            decoder: {
                readers: [decoder]
            },
            locate: true
        };

        console.log('Starting Quagga with config:', currentConfig);

        await new Promise((resolve, reject) => {
            Quagga.init(currentConfig, function (err) {
                if (err) {
                    console.error('Quagga init error:', err);
                    reject(err);
                    return;
                }
                quaggaInitialized = true;
                isScanning = true;
                Quagga.start();
                console.log('Quagga started successfully');
                resolve();
            });
        });

        Quagga.onDetected(function (result) {
            if (result.codeResult && result.codeResult.code) {
                const code = result.codeResult.code;
                console.log('Barcode detected:', code);
                dotNetHelper.invokeMethodAsync('OnBarcodeDetected', code)
                    .catch(error => console.error('Error invoking .NET method:', error));
            }
        });

    } catch (error) {
        console.error("Error starting Quagga:", error);
        throw error;
    }
}

export async function stopQuagga() {
    if (isScanning && typeof Quagga !== 'undefined') {
        try {
            Quagga.offDetected();
            Quagga.stop();
            quaggaInitialized = false;
            isScanning = false;
            console.log('Quagga stopped successfully');
        } catch (error) {
            console.error("Error stopping Quagga:", error);
        }
    }
}

export function getQuaggaStatus() {
    return {
        loaded: quaggaLoaded,
        initialized: quaggaInitialized,
        scanning: isScanning,
        available: typeof Quagga !== 'undefined',
        currentCamera: currentCameraId
    };
}