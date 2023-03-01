    let scanner = new Instascan.Scanner({ video: document.getElementById('preview') });

scanner.addListener('scan', function (content) {
    //Barkod okunduğunda veriyi burası yakalayacaktır.
    console.log(content)

    let inp = document.getElementById("qrCode")

    inp.value = content

});

let cameraList = []; //Tüm kameraları tutacağımız liste.

Instascan.Camera.getCameras().then(function (cameras, image) {
    //PC'de ki tüm kameraları algılayarak bizlere getirecektir.
    this.cameraList = cameras;
    cameras.forEach(element => {
        //Gelen kameralar select elementinde listelenmektedir.
        let cameraList = document.getElementById("cameras");
        let option = document.createElement("option");
        let res = document.createElement("response");
        option.text = element.id;
        option.value = element.id;
        cameraList.add(option);
        scanner.start(cameras[0])
    });
}).catch(() => console.error(e));

document.getElementById("cameras").addEventListener("change", event => {
    //Select elementinde seçilen kamerayı qr code scanner olarak belirleyen olaydır.
    scanner.start(this.cameraList.find(c => c.id == event.target.value));
})




