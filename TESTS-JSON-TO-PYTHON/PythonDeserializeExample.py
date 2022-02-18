# To use this code, make sure you
#
#     import json
#
# and then, to convert JSON from a string, do
#
#     result = welcome_from_dict(json.loads(json_string))

from dataclasses import dataclass
import json
from typing import Any, Optional, List, TypeVar, Callable, Type, cast

T = TypeVar("T")

def from_int(x: Any) -> int:
    return x


def from_str(x: Any) -> str:
    return x


def from_none(x: Any) -> Any:
    return x


def from_union(fs, x):
    for f in fs:
        try:
            return f(x)
        except:
            pass


def from_list(f: Callable[[Any], T], x: Any) -> List[T]:
    return [f(y) for y in x]


def to_class(c: Type[T], x: Any) -> dict:
    return cast(Any, x).to_dict()

@dataclass
class Awesomeobject:
    some_props1: int
    some_props2: str

    @staticmethod
    def from_dict(obj: Any) -> 'Awesomeobject':
        some_props1 = from_int(obj.get("SomeProps1"))
        some_props2 = from_str(obj.get("SomeProps2"))
        return Awesomeobject(some_props1, some_props2)

@dataclass
class User:
    id: int
    name: str
    created_at: str
    updated_at: str
    email: str
    testanadditionalfield: Optional[str] = None

    @staticmethod
    def from_dict(obj: Any) -> 'User':
        
        id = int(from_str(obj.get("id")))
        name = from_str(obj.get("name"))
        created_at = from_str(obj.get("created_at"))
        updated_at = from_str(obj.get("updated_at"))
        email = from_str(obj.get("email"))
        testanadditionalfield = from_union([from_str, from_none], obj.get("testanadditionalfield"))
        return User(id, name, created_at, updated_at, email, testanadditionalfield)

@dataclass
class Class1:
    id: int
    user_id: str
    awesomeobject: Awesomeobject
    created_at: str
    updated_at: str
    users: List[User]

    @staticmethod
    def from_dict(obj: Any) -> 'Class1':
        
        id = from_int(obj.get("id"))
        user_id = from_str(obj.get("user_id"))
        awesomeobject = Awesomeobject.from_dict(obj.get("awesomeobject"))
        created_at = from_str(obj.get("created_at"))
        updated_at = from_str(obj.get("updated_at"))
        users = from_list(User.from_dict, obj.get("users"))
        return Class1(id, user_id, awesomeobject, created_at, updated_at, users)

@dataclass
class Class2:
    some_property_of_class2: str

    @staticmethod
    def from_dict(obj: Any) -> 'Class2':
        
        some_property_of_class2 = from_str(obj.get("SomePropertyOfClass2"))
        return Class2(some_property_of_class2)

@dataclass
class Welcome:
    class1: Class1
    class2: Class2

    @staticmethod
    def from_dict(obj: Any) -> 'Welcome':
        
        class1 = Class1.from_dict(obj.get("Class1"))
        class2 = Class2.from_dict(obj.get("Class2"))
        return Welcome(class1, class2)

output = json.load(open('data.json'))
result = Welcome.from_dict(output)
result.class1.awesomeobject.some_props1
tets = result.class1.users[0].email
test = result