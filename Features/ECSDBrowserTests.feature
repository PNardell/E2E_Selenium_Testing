Feature: ECSD Browser Tests

Background: 

Given the test page "http://localhost:3000/" is showing

Scenario: Render The Challenge 
	When I click on the Render The Challenge button
	Then I see the Arrays Challenge

Scenario: Complete the Array Challenge
	When I get the centre cell of row 1 of the array challenge
	And I enter the index in field 1
	And I get the centre cell of row 2 of the array challenge
	And I enter the index in field 2
	And I get the centre cell of row 3 of the array challenge
	And I enter the index in field 3
	And I enter Paul Nardell into the name field
	And I submit the answers
	Then I see a completion message with the words Congratulations you have succeeded. Please submit your challenge ✅
	