# SharpChat
[![.NET Build](https://github.com/everharder/SharpChat/actions/workflows/dotnet-build.yml/badge.svg?branch=main)](https://github.com/everharder/SharpChat/actions/workflows/dotnet-build.yml)

SharpChat is a library for C# that offers the functionality of TypeChat, making it seamless to construct natural language interfaces using types.

Building natural language interfaces can be challenging. Traditional methods often rely on intricate decision trees to discern intent and gather necessary inputs for actions. The advent of Large Language Models (LLMs) has simplified this process by allowing natural language inputs from users to be matched with intents. However, this also brings its own set of challenges, such as constraining the model's reply for safety, structuring model responses for further processing, and ensuring the model's reply is valid. While prompt engineering aims to address these issues, it can be complex and fragile.

SharpChat introduces schema engineering as a replacement for prompt engineering.

Define types that represent the intents in your natural language application. This could range from a simple sentiment categorization interface to intricate types for applications like shopping carts or music. For instance, to incorporate more intents into a schema, developers can simply add types to a discriminated union. Hierarchical schemas can be achieved by using a "meta-schema" to select one or multiple sub-schemas based on user input.

With your types defined, SharpChat handles:

- Crafting a prompt for the LLM using types.
- Ensuring the LLM response adheres to the schema. If validation fails, the non-conforming output is repaired through further interactions with the language model.
- Concisely summarizing the instance without requiring an LLM, ensuring alignment with user intent.

With SharpChat, types are all you need!

## Getting Started

**Install SharpChat:**
(Provide installation instructions here)

You can also build SharpChat from the source:
(Provide build instructions here)

For a hands-on experience with SharpChat, explore the example projects.

## Like what you see?

(add buy me a coffee here)
