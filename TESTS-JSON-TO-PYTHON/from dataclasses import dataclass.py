from dataclasses import dataclass
from typing import Any, List
import json

@dataclass
class User:
    id: str
    name: str

    @staticmethod
    def from_dict(obj: Any) -> 'User':
     _id = int(obj.get("id"))
     _name = str(obj.get("name"))
     return User(_id, _name)

@dataclass
class Test:
    id: int
    userid: str
    users: List[User]

    @staticmethod
    def from_dict(obj: Any) -> 'Test':
        _id = int(obj.get("id"))
        _userid = str(obj.get("userid"))
        _users = [User.from_dict(y) for y in obj.get("users")]
        return Test(_id, _userid, _users)

@dataclass
class Root:
    Test: Test

    @staticmethod
    def from_dict(obj: Any) -> 'Root':
        _Test = Test.from_dict(obj.get("Test"))
        return Root(_Test)


# Load the json string to a variable
output = json.load(open('data.json'))
# Call the mapping function
result = Root.from_dict(output)
# Access your Python properties
result.Test.userid