javascript: (function () {
    $('.lister-item', $('.lister-list')).each(function () {
        var $this = $(this);
        var image = $('.lister-item-image', $this).data('tconst');
        var thisText = $('.lister-item-year', $this).text().replace('(', '').replace(')', '');
        var title = $('a', $('.lister-item-header', $this))[0].innerText.trim();

        if (thisText.indexOf("Video short") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("Video Game") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("TV Series") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("TV Mini-Series short") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("TV Mini-Series") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("Short") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }

        if (thisText.length >=4) {
            thisText = thisText.substr(0, 4);
        }

        var entry = {
            'imdb': '?imdb=' + image,
            'title': title + '.' + thisText + '.1080p.BluRay.x265-RARBG',
            'seeders': 20,
            'size': '1.88 GB',
            'isDocumentary': false
        };
        $.ajax({
            contentType: 'application/json;charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: 'http://localhost/rarbg.asmx/CheckMovie',
            data: JSON.stringify(entry),
            success: function (data) {
                $this.css(data.d.split(':')[0], data.d.split(':')[1]);
                if (data.d.split(':')[1] !== "#66ff66") {
                    console.log(entry.imdb.split('=')[1]);
                }
            },
            error: function (request, status, error) { console.log(request); }
        });
    });
})();