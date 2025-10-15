[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Dalion ValueObjects [<img src="https://dalion.eu/dalion128.png" align="right" width="48">](https://www.dalion.eu)

A C# code generator to cure my [Primitive Obsession](https://refactoring.guru/smells/primitive-obsession).

ValueObjects is a .NET Source Generator and analyzer. It turns your primitives (ints, decimals etc.) into value objects that represent domain concepts (CustomerId, AccountBalance etc.).

Please see the [Wiki](wiki) for more detailed information, such as getting started, tutorials, and how-tos.

## Give a Star! :star:
If you like or are using this project please give it a star. Thanks!

## Motivation

Creating value objects is a repetitive task. This project aims to reduce the amount of boilerplate code that needs to be written, by generating it.

Inspired by [Vogen](https://github.com/SteveDunn/Vogen).

### Limitations Overcome

This library addresses several limitations found in similar solutions, such as Vogen:

- **No Run-Time Dependency:**  
  Unlike e.g. Vogen, which requires `Vogen.SharedTypes.dll` at run-time, all necessary types are generated alongside your value objects\. There are no additional run-time dependencies\. All required types are generated at compile-time and generated code is self-contained\.

- **Automatic Generation of Pre-Set Values:**  
  Every value object automatically gets an `Empty` or `Default` (configurable) pre-set value, reducing boilerplate and ensuring consistency\.

### Fundamental Differences in Approach

This library takes a distinct approach to value objects, with perspectives and assumptions that are hard to align with solutions like Vogen.

- **Comparison of Uninitialized Value Objects:**  
  \- *Vogen:* Treats uninitialized value objects as different from each other\.  
  \- *This Library:* Considers uninitialized value objects of a type to be equal\.

- **Serialization of Uninitialized Value Objects:**  
  \- *Vogen:* Cannot serialize uninitialized value objects\.  
  \- *This Library:* Supports serialization and round-tripping of uninitialized value objects\.

- **Deserialization of Pre-Set Values:**  
  \- *Vogen:* Only supports deserialization of pre-set values declared via attributes\.  
  \- *This Library:* Can deserialize any pre-set value declared as a `public static readonly` field, even if it does not pass validation\.

- **Validation and Error Reporting:**  
  \- *Vogen:* Validation information is private and inaccessible\.  
  \- *This Library:* Exposes validation status and error messages, allowing you to check if a value object is valid and retrieve the reason if not\. This enables creation of pre-set, initialized, but invalid values\.

These assumptions and approaches reflect my personal perspective on value objects, which may differ from others. For me, they make this library more practical and usable in real-world projects.

---
"Anybody can make something that works. Software craftsmanship is the ability to keep it understandable, maintainable and extensible."