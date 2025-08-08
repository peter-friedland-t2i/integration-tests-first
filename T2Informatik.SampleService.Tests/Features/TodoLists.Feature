Feature: Todo Lists

This feature is about handling todo lists for different users, including sharing.
    
Background: 
    Given The system has the following users with the following owned todo lists
    | UserName | Owned Todo Lists                 |
    | Theo     | "Theos Todos 1", "Theos Todos 2" |
    | Tina     |                                  |

Rule: Owned Todos are only for the owners

Scenario: Create a new todo
    When "Tina" has created the todo list "Tinas Todos"
    Then "Theo" has the todo lists "Theos Todos 1" and "Theos Todos 2"
    And "Tina" has the todo list "Tinas Todos"
    
Rule: Shared todos are visible for shared users.
    
Scenario: Share todo with a user
    When "Theo" shares "Theos Todos 1" with "Tina"
    Then "Theo" has the overall todo lists
      | Name          | Shared From |
      | Theos Todos 1 |             |
      | Theos Todos 2 |             |
    And "Tina" has the overall todo lists
      | Name          | Shared From |
      | Theos Todos 1 | Theo        |