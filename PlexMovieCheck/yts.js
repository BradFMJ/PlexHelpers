javascript: (function () {
    $('div.browse-movie-bottom').each(function () {
        var $this = $(this);
        var entry = {
            'title': $('a', $this)[0].innerText,
            'year': parseInt($('.browse-movie-year', $this)[0].innerText, 10)
        };
        $.ajax({
            contentType: 'application/json;charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: 'http://localhost/yts.asmx/CheckMovie',
            data: JSON.stringify(entry),
            success: function (data) { $this.css(data.d.split(':')[0], data.d.split(':')[1]); },
            error: function (request, status, error) { console.log(request); }
        });
    });
})();


