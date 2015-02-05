casper.test.begin('Iniside helloy test', 4, function suite(test) {
    var initialClinetsMatchingNewName;

    casper.start("http://localhost:58219/Client", function () {
        test.assertTitle("Kunder", "navigate to Client Page Succesfull");
     
        initialClinetsMatchingNewName = this.evaluate(function () {
            return $('a:contains("Test Client From Acceptance test")').length;
        });

        this.clickLabel('Skapa ny kund', 'a');
    });
    
    casper.then(function() {
           
        test.assertTitle("Skapa nya kund", "navigate to create new Client Page Succesfull");

        var consultantId = this.evaluate(function() {
            return $('option:contains("Peter Weibull")').val();
        });
            
        this.fillSelectors('form[action="/client/create"]',
            {
                'input[name="Name"]': 'Test Client From Acceptance test',
                'input[id="isActive"]': true,
                'select[name="ConsultantId"]': consultantId,
                
            },true);
    });
    
    casper.waitForText("Aktiva kunder",(function() {
       
        test.assertTitle("Kunder", "navigate to Client Page after creating new client Succesfull");
        
        var numClinetsMatchingNewName = this.evaluate(function ()
        {
            return $('a:contains("Test Client From Acceptance test")').length;
        });
        
        test.assert(numClinetsMatchingNewName == initialClinetsMatchingNewName + 1, "The new Client was added successfully");
        
    });


    casper.run(function() {
        test.done();
    });
});