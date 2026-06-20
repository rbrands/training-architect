# Stage 2: Implementation Guide for AI Agents

This document provides detailed implementation instructions for AI agents executing Stage 2: Project Detection.

---

## STEP 1: Check for Existing Project

```pseudocode
workspace_root = get_current_workspace_path()  // e.g., d:\skills\blazorui-builder

// Search for project files
has_csproj = file_exists(workspace_root + "/*.csproj")
has_app_razor = file_exists(workspace_root + "/Components/App.razor") OR 
                file_exists(workspace_root + "/Pages/App.razor")
has_program_cs = file_exists(workspace_root + "/Program.cs")

project_exists = has_csproj OR (has_app_razor AND has_program_cs)

if project_exists:
    proceed_to_AUTO_DETECT_FLOW()
else:
    proceed_to_CREATE_NEW_PROJECT_FLOW()
```

---

## STEP 2A: AUTO-DETECT FLOW (Project Exists)

```pseudocode
1. Use file_search to find *.csproj file
2. Read .csproj file → Extract:
   - TargetFramework (e.g., net8.0, net9.0, net10.0)
   - Project type hints (Blazor WebAssembly vs Web App)
   - Syncfusion package versions (if any)

3. Read Program.cs → Detect:
   - builder.Services.AddRazorComponents() = WebApp
   - builder.Services.AddScoped<HttpClient>() = WASM
   - Syncfusion.Licensing.RegisterLicense() call

4. Check wwwroot/index.html exists = WASM indicator
5. Check Components/App.razor exists = WebApp indicator

6. Auto-detect CSS framework by checking:
   - tailwind.config.js → Tailwind CSS
   - bootstrap files → Bootstrap 5
   - Default to scoped .razor.css

7. Read .editorconfig if exists → formatting rules

8. Check appsettings.json for SYNCFUSION_LICENSE_KEY

RETURN: detected_settings object with all fields populated
```

---

## STEP 2B: CREATE NEW PROJECT FLOW (No Project Detected)

### User Decision 1: Should we create a new project?

```pseudocode
response = ask_user({
  question: "❌ No Blazor project detected in this workspace.\n\nWould you like to create a new Blazor application?",
  options: [
    { label: "Yes, create new project", value: "yes" },
    { label: "No, cancel (I'll set up manually)", value: "no" }
  ]
})

if response == "no":
    print("You'll need to create a Blazor project manually first.")
    print("Commands to create manually:")
    print("  dotnet new blazor --name MyApp")
    print("  cd MyApp")
    print("  dotnet add package Syncfusion.Blazor")
    CANCEL_WORKFLOW()

// response == "yes"
proceed_to_SELECT_BLAZOR_TYPE()
```

### User Decision 2: Which Blazor type? ⭐ CRITICAL

```pseudocode
blazor_choice = ask_user({
  question: "Which Blazor project type would you like to create?\n\n" +
            "[1] BlazorWASM (WebAssembly)\n" +
            "    └─ Client-side only\n" +
            "    └─ Themes loaded in wwwroot/index.html\n" +
            "    └─ Better for: SPAs, offline-first\n\n" +
            "[2] BlazorWebApp (Server/Auto-Render)\n" +
            "    └─ Full-stack with server backend\n" +
            "    └─ Themes loaded in Components/App.razor\n" +
            "    └─ Better for: Complex apps, real-time features\n\n" +
            "Which one?",
  options: [
    { label: "BlazorWASM (Client-side)", value: "wasm" },
    { label: "BlazorWebApp (Server/Auto-Render)", value: "webapp" }
  ]
})

STORE: user_blazor_choice = blazor_choice
proceed_to_CREATE_PROJECT()
```

### Step 2B-3: Create Project Using dotnet CLI

```pseudocode
project_name = "BlazorApp"  // Default name
workspace_root = get_current_workspace_path()

if user_blazor_choice == "wasm":
    command = "dotnet new blazor --name " + project_name + " --empty"
else:  // webapp
    command = "dotnet new blazor --name " + project_name + " --interactivity server --empty --all-interactive"

PRINT("Creating new project...\n")
PRINT("Command: " + command + "\n")

// Run in terminal (use run_in_terminal tool with isBackground=false)
result = run_in_terminal({
    command: command,
    goal: "Create new Blazor project",
    isBackground: false,
    timeout: 60000
})

if result.exit_code != 0:
    PRINT("❌ Project creation failed: " + result.output)
    ask_user("Would you like to try again or cancel?")
    if response == "try_again":
        retry_create_project()
    else:
        CANCEL_WORKFLOW()

PRINT("✓ Project created successfully\n")

// Install Syncfusion Blazor package in new project
PRINT("Installing Syncfusion.Blazor package...\n")
install_result = run_in_terminal({
    command: "dotnet add package Syncfusion.Blazor --project " + project_name,
    goal: "Install Syncfusion Blazor components",
    isBackground: false,
    timeout: 60000
})

if install_result.exit_code != 0:
    PRINT("⚠ Syncfusion package installation failed")
    PRINT("You may need to install it manually later")
else:
    PRINT("✓ Syncfusion.Blazor installed\n")

STORE: new_project_path = workspace_root + "/" + project_name
proceed_to_AUTO_DETECT_FLOW()  // Re-run detection on new project
```

### Step 2B-4: Auto-Detect Settings from New Project

After project creation, run the AUTO-DETECT FLOW (Step 2A) on the new project directory.

The system will auto-populate all settings based on the newly created project structure.

---

## STEP 3: Present Settings to User

```pseudocode
if project_is_new:
    header = "✓ New Blazor Project Created!"
else:
    header = "✓ Project Detected"

settings_summary = """
{header}

Framework & Platform:
✓ .NET Version: {detected_settings.dotnet_version}
✓ Blazor Project Type: {detected_settings.blazor_type} (WASM or WebApp)
✓ Theme CSS Location: {detected_settings.theme_css_location}

Code & Formatting:
✓ Language: C# with Razor syntax
✓ Component Directory: {detected_settings.component_dir}
✓ Formatting: {detected_settings.formatting_rules}

Styling:
✓ CSS Strategy: {detected_settings.css_framework}

Syncfusion:
✓ Syncfusion Version: {detected_settings.syncfusion_version}
✓ License Status: {detected_settings.license_status}
"""

PRINT(settings_summary)

user_response = ask_user({
  question: "Proceed with these settings?",
  options: [
    { label: "✓ Confirm", value: "confirm" },
    { label: "Override Settings", value: "override" },
    { label: "Cancel", value: "cancel" }
  ]
})

if user_response == "confirm":
    RETURN detected_settings
    proceed_to_STAGE_3()
    
elif user_response == "override":
    // Allow user to change specific settings
    // This could include dotnet version, syncfusion version, etc.
    proceed_to_OVERRIDE_FLOW()
    
else:  // cancel
    CANCEL_WORKFLOW()
```

---

## Tools to Use in Stage 2

| Task | Tool | Usage |
|------|------|-------|
| Search for `.csproj` | `file_search` | `file_search({"query": "**/*.csproj"})` |
| Search for `.razor` files | `file_search` | `file_search({"query": "**/App.razor"})` |
| Read `.csproj` content | `read_file` | Parse XML to extract properties |
| Search for strings in files | `grep_search` | Find "AddRazorComponents" in Program.cs |
| Ask user questions | `ask_questions` | Present multiple choice decisions |
| Run dotnet commands | `run_in_terminal` | Create projects, install packages |
| Check terminal output | `get_terminal_output` | Verify command success |
| List directory contents | `list_dir` | Confirm project structure created |

---

## Summary

**Stage 2 now has two flows:**

1. **Original Flow (Project Exists):**
   - Detect settings
   - Ask user to confirm/override
   - Continue to Stage 3

2. **New Flow (No Project Exists):**
   - Ask: Create new project?
   - Ask: WASM or WebApp? ⭐ CRITICAL
   - Run: `dotnet new blazor ...`
   - Run: `dotnet add package Syncfusion.Blazor`
   - Detect settings from new project
   - Ask user to confirm/override
   - Continue to Stage 3

**Result:** Users can now start the Blazor UI builder from a completely empty workspace and have a working project created for them, ready for component generation.

**Next Step:** After Stage 2 completes, proceed to **Stage 3: Layout Analysis & Component Mapping**.
