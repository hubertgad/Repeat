$('body').on('keydown', 'textarea', function (e) {
    if (e.which == 9) {
        e.preventDefault();
        var start = this.selectionStart;
        var end = this.selectionEnd;

        $(this).val($(this).val().substring(0, start)
            + "\t"
            + $(this).val().substring(end));

        this.selectionStart = this.selectionEnd = start + 1;
    }
});