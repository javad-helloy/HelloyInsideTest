The tests in the Tests folder are automated tests to determine if the whole system works accordingly after changes.

To run tests first install PhantomJS and CasperJS from:

PhantomJS: http://phantomjs.org/download.html

CasperJS: http://casperjs.org/ (https://github.com/n1k0/casperjs/zipball/1.1-beta1)

By executing the command below from the Automated tests folder all the tests in the Tests Sub-folder will be executed:

casperjs test Tests --pre=Login.js