// دسترسی به دوربین
function openCamera() {
    navigator.mediaDevices.getUserMedia({ video: true })
        .then(stream => {
            const video = document.querySelector('video');
            video.srcObject = stream;
        })
        .catch(error => {
            console.error("Error accessing camera: ", error);
        });
}

// دسترسی به لوکیشن
function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(position => {
            console.log("Latitude: " + position.coords.latitude);
            console.log("Longitude: " + position.coords.longitude);
        }, error => {
            console.error("Error accessing location: ", error);
        });
    } else {
        console.error("Geolocation is not supported by this browser.");
    }
}



//تشخیص صفحه نمایش موبایل
window.isMobile = function () {
    return window.innerWidth <= 768;
};  

