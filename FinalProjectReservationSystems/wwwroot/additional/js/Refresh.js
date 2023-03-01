$(document).on("submit", "#form", function (event) {
    event.preventDefault(); // Sayfa yenilenmesini engellemek için
    var form = $(this)
    // Ajax çağrısı
    $.ajax({
        url: $(this).attr('action'), // Formun post edileceği URL
        type: "POST", // Formun post edileceği metot (POST veya GET)
        data: $(this).serialize(), // Form verileri
        success: function (response) {
            $(".comment-option").append(
                `<div class="single-comment-item second-comment ">
                                <div class="sc-author">
                                    <img src="/uploads/profilPhotos/${response.profilPhoto}" alt="">
                                </div>
                                <div class="sc-text">
                                    <span>${response.date.substring(0, 16).replace("T", "  ") }</span>
                                    <h5>${response.fullname}</h5>
                                    <p>
                                        ${response.text}
                                    </p>                              
                                </div>
                            </div>`),
                form[0].reset();
        },
        error: function (error) {
            console.log(error);
        }
    });
});




















$(document).on("submit", ".reply-comment", function (event) {
    event.preventDefault();
    var form = $(this)
    $.ajax({

        url: $(this).attr('action'),
        type: "POST",
        data: $(this).serialize(),
        success: function (response) {
            debugger
            $("#comment" + response.commentId).append(`
                
                <div class="single-comment-item reply-comment">
                                        <div class="sc-author">
                                            <img src="/uploads/profilPhotos/${response.profilPhoto}" alt="">
                                        </div>
                                        <div class="sc-text">
                                            <span>${response.date.substring(0, 16).replace("T", "  ") }</span>
                                            <h5>${response.fullname}</h5>
                                            <p>
                                                ${response.text}
                                            </p>
                                        </div>
                                    </div>
                
            `)
            form[0].reset();
            console.log(response);
        },
        error: function (error) {
            console.log(error);
        }
    });

});