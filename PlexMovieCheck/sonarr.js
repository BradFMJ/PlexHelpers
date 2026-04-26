javascript: (function () {
    var rows = document.querySelectorAll('div.VirtualTableRow-row-cfNxh'); rows.forEach(function (row) {
        var sourceInput = row.querySelector('.CheckInput-checkbox-WEQ3S');
        if (sourceInput.checked) {
            var suggestedText = row.querySelector('.ImportSeriesTitle-title-iHFav');
            var suggested = suggestedText.textContent;
            var suggestedYear = row.querySelector('.ImportSeriesTitle-year-VsT7i');
            if (suggestedYear && suggestedYear.textContent) {
                suggested += ' ' + suggestedYear.textContent;
            }
            console.log(sourceInput.name + '=' + suggested);
            if (normalizeAlphaNum(sourceInput.name) == normalizeAlphaNum(suggested)) {
                row.style.backgroundColor = 'lightgreen';
            }
            else {
                row.style.backgroundColor = 'red';
                sourceInput.checked = false;
                var input = (sourceInput instanceof HTMLInputElement)
                    ? sourceInput
                    : (sourceInput && sourceInput.querySelector && sourceInput.querySelector('input[type="checkbox"],input[type="radio"]'));

                if (input) {
                    try {
                        input.click();
                        if (input.checked) {
                            input.checked = false;
                            input.dispatchEvent(new Event('input', { bubbles: true }));
                            input.dispatchEvent(new Event('change', { bubbles: true }));
                        }
                    } catch (e) {
                        input.checked = false;
                        input.dispatchEvent(new Event('input', { bubbles: true }));
                        input.dispatchEvent(new Event('change', { bubbles: true }));
                    }
                } else {
                    row.style.backgroundColor = 'red';
                    sourceInput && sourceInput.classList && sourceInput.classList.remove('checked');
                }
            }
        }
    });
    function normalizeAlphaNum(s) {
        return String(s || '').replace(/[^a-zA-Z0-9]/g, '').toLowerCase();
    }
})();