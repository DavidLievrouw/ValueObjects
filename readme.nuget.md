[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/Dalion.ValueObjects)](https://www.nuget.org/packages/Dalion.ValueObjects/)
[![Build status](https://ci.appveyor.com/api/projects/status/nrvheog39e1xy4ge?svg=true)](https://ci.appveyor.com/project/DavidLievrouw/valueobjects)
[![Tests status](https://img.shields.io/appveyor/tests/DavidLievrouw/valueobjects?compact_message)](https://ci.appveyor.com/project/DavidLievrouw/valueobjects)
[![Last commit](https://img.shields.io/github/last-commit/DavidLievrouw/ValueObjects)](https://github.com/DavidLievrouw/ValueObjects)
[![Stack](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=.net&logoColor=white)](https://dotnet.microsoft.com)

# Dalion ValueObjects

A C# code generator to cure [Primitive Obsession](https://refactoring.guru/smells/primitive-obsession).

ValueObjects is a .NET Source Generator and analyzer. It turns your primitives (ints, decimals etc.) into [value objects](https://en.wikipedia.org/wiki/Value_object) that represent domain concepts (`CustomerId`, `AccountBalance`, etc.).

## Installation and usage

Add the [Dalion.ValueObjects](https://www.nuget.org/packages/Dalion.ValueObjects/) NuGet package to your project.

```xml
<ItemGroup>
  <PackageReference
    Include="Dalion.ValueObjects"
    Version="[1.0.0,)"
    ReferenceOutputAssembly="false"
    OutputItemType="Analyzer"
  />
</ItemGroup>
```

Then, create a `readonly partial record struct` and annotate it with the `[ValueObject<T>]` attribute:

```csharp
using System;~~~~

[ValueObject<Guid>]
public readonly partial record struct CustomerId;
```

This enables the following for the type:

- Guarding against creation of uninitialized or invalid instances in code (based on validation rules you define)
- Ability to normalize the underlying primitive value upon creation
- `String` representation based on the underlying primitive value
- `System.Text.Json` Serialization and deserialization support
- `TypeConverter` support for conversion to and from the underlying primitive value
- Ability to check if an instance is initialized and valid, and to retrieve validation error messages
- Ability to define pre-set values as `public static readonly` fields (even if they are considered to be invalid)
- Implementation of `IComparable` and `IComparable<T>`
- Implementation of `IFormattable` for formatting support
- Implementation of `IParsable<T>` for parsing support
- Implementation of `ISpanParsable<T>` for span parsing support, and `IUtf8SpanParsable<T>` for UTF8 span parsing support (if the underlying type supports it)
- Generation of a static property `Default` (name is configurable) that represents the default value for the value object type

It makes it possible to enable the following for the type, using arguments in the attribute to configure behavior:

- Equality comparison based on the underlying primitive value (configurable to be case-sensitive or case-insensitive for `string`-based value objects)
- Equality operators (`==` and `!=`) based on the underlying primitive value
- Implicit conversion to and from the underlying primitive value
- Optional generation of helper extension methods for [FluentValidation](https://docs.fluentvalidation.net) integration
- Optional generation of helper extension methods to facilitate creation (e.g. `32.Celsius()`)

This library also enables multiple roslyn analyzers that validate correct usage of the `[ValueObject<T>]` attribute and the annotated types.

You can examine the generated code in the [generated sample code files](/DavidLievrouw/ValueObjects/tree/main/src/Dalion.ValueObjects.Samples/Generated/Dalion.ValueObjects/Dalion.ValueObjects.Generation.ValueObjectGenerator). The behavior of the generated types is documented by [unit tests](https://github.com/DavidLievrouw/ValueObjects/tree/main/src/Dalion.ValueObjects.Tests/Samples).

Similarly, the behavior of the roslyn analyzers is documented by [unit tests](https://github.com/DavidLievrouw/ValueObjects/tree/main/src/Dalion.ValueObjects.Rules.Tests).

See the [Wiki](https://github.com/DavidLievrouw/ValueObjects/wiki) for more detailed information, such as getting started, tutorials, and how-tos.

## Give a Star! :star:
If you like or are using this project please give it a star. Thanks!

## Motivation

Creating value objects is a repetitive task. This project aims to reduce the amount of boilerplate code that needs to be written, by generating it.

Inspired by the awesome [Vogen](https://github.com/SteveDunn/Vogen) project.

### Limitations Overcome

This library addresses several limitations found in similar solutions, such as [Vogen](https://github.com/SteveDunn/Vogen) or [StronglyTypedId](https://github.com/andrewlock/StronglyTypedId):

- **No Run-Time Dependency:**  
  Unlike e.g. [Vogen](https://github.com/SteveDunn/Vogen), which requires an assembly at run-time (e.g. `Vogen.SharedTypes.dll`), all necessary types are generated alongside your value objects\. There are no additional run-time dependencies\. All required types are generated at compile-time and generated code is self-contained\.

- **Automatic Generation of Pre-Set Values:**  
  Every value object automatically gets an `Empty` or `Default` (configurable) pre-set value, reducing boilerplate and ensuring consistency\.

### Fundamental Differences in Approach

This library takes a distinct approach to [value objects](https://en.wikipedia.org/wiki/Value_object), with perspectives and assumptions that are incompatible with those of [Vogen](https://github.com/SteveDunn/Vogen) or [StronglyTypedId](https://github.com/andrewlock/StronglyTypedId).

These assumptions and approaches reflect our perspective on [value objects](https://en.wikipedia.org/wiki/Value_object), which may differ from others. For us, they make this library more practical and usable in real-world projects.
