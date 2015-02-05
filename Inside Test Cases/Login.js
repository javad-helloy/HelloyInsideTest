casper.test.begin('Iniside helloy test', 3, function suite(test) {
    casper.start("http://localhost:58219/", function () {
        test.assertTitle("Logga in på Helloy Inside", "login homepage title is the one expected");
        test.assertExists('form[action="/account/logon"]', "login form is found");
        this.fill('form[action="/account/logon"]',
            {
                'UserName': 'peter',
                'Password': 'test12'
            },true);
    });

    casper.then(function() {
        test.assertTitle("Kunder", "login Succesfull");
    });

    casper.run(function() {
        test.done();
    });
});