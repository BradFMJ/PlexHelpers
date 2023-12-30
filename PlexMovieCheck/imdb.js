javascript: (function () {
    $('.filmo-row', $('#filmo-head-actor').next()).each(function () {
        var $this = $(this);
        var thisText = $this.text();
        var year = $('.year_column', $this)[0].innerText.trim();
        var title = $('a', $this)[0].innerText.trim();
        if (year.indexOf("/") > -1) {
            year = year.split("/")[0];
        }
        if (thisText.indexOf("(Video short)") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("(Video Game)") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("(TV Series)") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("(TV Series Short)") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("(TV Mini-Series short)") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("(TV Mini-Series)") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("(TV Mini Series)") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("(TV Series documentary)") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("(Music Video short)") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        if (thisText.indexOf("(Short)") > -1) {
            $this.css("background-color", "#cccccc");
            return;
        }
        var entry = {
            'imdb': '?imdb='+$('a', $this)[0].attributes.href.value.split('/')[2],
            'title': title + '.' + year + '.1080p.BluRay.x265-RARBG',
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
            success: function(data) {
                $this.css(data.d.split(':')[0], data.d.split(':')[1]);
                if (data.d.split(':')[1] !== "#66ff66") {
                    console.log(entry.imdb.split('=')[1]);
                }
            },
            error: function (request, status, error) { console.log(request); }
        });
    });
})();

javascript: (function () {
    $('.filmo-row', $('#filmo-head-actor').next()).each(function () {
        var $this = $(this);
        var entry = {
            'imdb': 'imdb=' + $('a', $this)[0].attributes.href.value.split('/')[2],
            'title': $('a', $this)[0].innerText + '' + $('.year_column', $this)[0].innerText,
            'seeders': 20,
            'size': '1.88 GB',
            'isDocumentary': false
        };
        console.log(JSON.stringify(entry));
    });
})();