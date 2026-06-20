# Stage 1: Intent Analysis

**Purpose:** Parse and validate the user's natural language request, identify component type and modifiers, resolve ambiguities.

**AI Should:**
- Read the user's raw query carefully
- Identify primary intent: `generate_component`, `generate_page`, or `modify_existing`
- Extract component type (e.g., "login form" → LoginForm.razor, "data grid" → DataGrid.razor)
- Extract modifiers (e.g., "with Syncfusion DataGrid" → feature:syncfusion-datagrid, "dark theme" → styling:dark)
- Identify target directory if specified (e.g., "in the Auth folder" → targetDir:Components/Auth/)

**Ambiguity Resolution:**
If the request is unclear, ask ONE clarifying question. Examples:

| Ambiguous Input | Clarifying Question |
|---|---|
| "Build me a form" | "What kind of form? (login, registration, contact, multi-step)" |
| "Add a component" | "What component would you like? (data table, navigation, modal, etc.)" |
| "Make it better" | "Which component and what aspect? (accessibility, styling, layout)" |

**Output to User:**
One-line confirmation:
```
✓ Understood: Generating a dark-themed LoginForm.razor component with Syncfusion authentication support.
Starting project detection...
```

**Status:** This stage requires NO user interaction for confirmation. AI decides intent based on pure reasoning.
