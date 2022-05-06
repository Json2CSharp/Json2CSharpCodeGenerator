from typing import List
from typing import Any
from dataclasses import dataclass
import json
@dataclass
class Object:
    prop1: int
    prop2: str

    @staticmethod
    def from_dict(obj: Any) -> 'Object':
        _prop1 = int(obj.get("prop1"))
        _prop2 = str(obj.get("prop2"))
        return Object(_prop1, _prop2)

@dataclass
class User:
    id: str
    name: str

    @staticmethod
    def from_dict(obj: Any) -> 'User':
        _id = str(obj.get("id"))
        _name = str(obj.get("name"))
        return User(_id, _name)

@dataclass
class Test:
    id: int
    userid: str
    object: Object
    created_at: str
    updated_at: str
    users: List[User]

    @staticmethod
    def from_dict(obj: Any) -> 'Test':
        _id = int(obj.get("id"))
        _userid = str(obj.get("userid"))
        _object = Object.from_dict(obj.get("object"))
        _created_at = str(obj.get("created_at"))
        _updated_at = str(obj.get("updated_at"))
        _users = [User.from_dict(y) for y in obj.get("users")]
        return Test(_id, _userid, _object, _created_at, _updated_at, _users)

@dataclass
class Test2:
    Prop2: str

    @staticmethod
    def from_dict(obj: Any) -> 'Test2':
        _Prop2 = str(obj.get("Prop2"))
        return Test2(_Prop2)

@dataclass
class Root:
    Test: Test
    Test2: Test2

    @staticmethod
    def from_dict(obj: Any) -> 'Root':
        _Test = Test.from_dict(obj.get("Test"))
        _Test2 = Test2.from_dict(obj.get("Test2"))
        return Root(_Test, _Test2)

# Example Usage
# jsonstring = json.loads(myjsonstring)
# root = Root.from_dict(jsonstring)

# Load the json string to a variable
output = json.load(open('data.json'))
# Call the mapping function
result = Root.from_dict(output)
# Access your Python properties
result.Test.created_at

#test = result.Class1.awesomeobject.SomeProps1
# test2 = result.Class1.users[0].email
test3 = 2
