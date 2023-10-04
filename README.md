# SharpChat
[![.NET Build](https://github.com/everharder/SharpChat/actions/workflows/dotnet-build.yml/badge.svg?branch=main)](https://github.com/everharder/SharpChat/actions/workflows/dotnet-build.yml)

SharpChat is a C# library that tries to offer functionality similar to [TypeChat](https://github.com/microsoft/TypeChat), making it seamless to construct natural language interfaces using types.

Building natural language interfaces can be challenging. Traditional methods often rely on intricate decision trees to discern intent and gather necessary inputs for actions. The advent of Large Language Models (LLMs) has simplified this process by allowing natural language inputs from users to be matched with intents. However, this also brings its own set of challenges, such as constraining the model's reply for safety, structuring model responses for further processing, and ensuring the model's reply is valid. While prompt engineering aims to address these issues, it can be complex and fragile.

SharpChat introduces schema engineering as a replacement for prompt engineering.

Define types that represent the intents in your natural language application. This could range from a simple sentiment categorization interface to intricate types for applications like shopping carts or music. For instance, to incorporate more intents into a schema, developers can simply add types to a discriminated union.

## Goals

- Creating an API for easy schema definition using delegate types.
- Ensuring the LLM response adheres to the schema. If validation fails, the non-conforming output is repaired through further interactions with the language model. (TODO)
- Concisely summarizing the instance without requiring an LLM, ensuring alignment with user intent. (TODO)

## Usage

TODO

## Get Started

Take a look at the `Examples` folder for inspiration on how to use SharpChat.
