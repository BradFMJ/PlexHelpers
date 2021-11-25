javascript: (function () {
    $('tr.lista2').each(function () {
        var $this = $(this);
        var entry = {
            'imdb': $('a', $this).length > 2 ? $('a', $this)[2].search : '',
            'title': $('a', $this)[1].innerText,
            'seeders': parseInt($('td', $this)[4].innerText, 10),
            'size': $('td', $this)[3].innerText,
            'isDocumentary': $('span', $('td', $this)[1]).text().indexOf('Documentary')>-1 ? true : false
        };
        $.ajax({
            contentType: 'application/json;charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: 'http://localhost/rarbg.asmx/CheckMovie',
            data: JSON.stringify(entry),
            success: function (data) { $this.css(data.d.split(':')[0], data.d.split(':')[1]); },
            error: function (request, status, error) { console.log(request); }
        });
    });
})();


