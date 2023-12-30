javascript: (function () {
    $('div.tgxtablerow').each(function () {
        var $this = $(this);
        var divs = $('div', $this);
        var $title, title, $seeders, seeders, $size, size, $imdb;
        if (divs.length === 14) {
            $title = divs[3];
            $size = divs[8];
            $seeders = divs[11];

            title = $('a.txlight', $title).text();
            seeders = $('font', $seeders)[0].innerText;
            size = $('span', $size).text();

            var $as = $('a', $title);
            if ($as.length === 3) {
                $imdb = $('a', $title)[2];
            }
            else {
                $imdb = $('a', $title)[1];
            }
        }
        else if (divs.length === 12) {
            $title = divs[1];
            $size = divs[6];
            $seeders = divs[9];

            title = $('a.txlight', $title).text();
            seeders = $('font', $seeders)[1].innerText;
            size = $('span', $size).text();

            var $as = $('a', $title);
            if ($as.length === 3) {
                $imdb = $('a', $title)[2];
            }
            else {
                $imdb = $('a', $title)[1];
            }
        }
        else {

        }
        if ($imdb) {
            var imdb = $imdb.search;
            var entry = {
                'imdb': imdb,
                'title': title,
                'seeders': parseInt(seeders, 10),
                'size': size,
                'isDocumentary': false
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
        }
    });
})();


