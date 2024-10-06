Feature: Credit Card Payment

  Scenario: Successful credit card payment
    Given I am at the credit card payment page
    When I click to pay with valid info
    Then my transaction is successful
