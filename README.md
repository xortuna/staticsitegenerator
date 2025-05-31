# Static Site Generator in C# #

Dirt simple static site generator

Used to generate the https://pisteapp.com website

## Features

* HTML Templating
* Markdown content
* Asset copying
* Easy to use Functions and macros to automate templated elements
* Context aware variable system  
* Automatic image resolution selection/webp fallback

## Command Line

```
cd mysite
StaticSiteGenerator.exe -w -x "https://mysite.com"
cd _www
python3 -m http.server
```

## Arguments
| Argument      | Description      |
| ------------- | ------------- |
| -w | Watch the input folder for changes |
| -mi | Dont auto detect multi resolution images |
| -t | Add additional asset file types (DEF: ".css", ".png", ".svg", ".js", ".webp", ".mp4", ".webm") |
| -x "https://mysite.com" | Set the Base URL and generate a sitemap.xml |

## Templates

The parser supports both HTML and Markdown files. Each file is pre-processed for template tags before being copied to the output folder.
Files and folders starting with an underscore are not copied automatically but can be referenced. Theres a global template directory called _partial you can use. 

Example Structure for a blog
```
/_partial/_header.html         Includes our CSS and JS
/_partial/_footer.html         Includes our copyright etc
/_partial/_navbar.html         Includes our navigation bar and menu
/assets/css/                   Contains asset files that are copied
/assets/js/                    Contains asset files that are copied
/assets/image/                 Contains asset files that are copied 
/blog/index.html               Page showing a list of blog posts
/blog/posts/__template.html    The HTML template the blog posts should use
/blog/posts/mypost1.md         A blog post content
/blog/posts/mypost2.md         A blog post content
```

When running the tool a new folder called _www will be created, any assets or HTML files will be copied into the relevant directories, and the Markdown files will be parsed and exploded into subdirectories

```
_www/assets/css/
_www/assets/js/
_www/assets/image/
_www/blog/index.html
_www/blog/posts/mypost1/index.html
_www/blog/posts/mypost2/index.html
```

# Templating Engine
The templating engine has a rather crude and simplistic function resolving system. Templates can be used in all HTML files.

The most common syntax you'll use is the include("myhtml.html") function to embed other html files into your html file.

Basic Syntax
```
{{include('myfile.html')}}
```

Nested Functions are evaluated automatically

Function Syntax
```
The answer is: {{add(multiply(2, 2),10)}}

The answer is: 14

```

You can also use LINQ style syntax where the left hand parameter is deduced from the preceding elements

Linq Syntax
```
{{list_files('/blog/posts','*.md').take(5)}}

Is equivalent to:
{{take(list_files('/blog/posts','*.md'), 5)}}

```

When including files, any assigned variables are still in scope of the included file.

Here we are highlighting the current navbar item based on the current directory of the page that *included* the navbar
```HTML
<div class="collapse navbar-collapse" id="navbarNav">
	<ul class="navbar-nav ms-auto">
    <li class="nav-item">
      <a class="nav-link {{if(equal(var('directory.path'),''),'active','')}}" href="/">
        <span>Home</span>
      </a>
    </li>
    <li class="nav-item">
      <a class="nav-link {{if(starts_with(var('directory.path'),'/features'),'active')}}" href="/features/">
        <span>Features</span>
      </a>
    </li>
    <li class="nav-item">
      <a class="nav-link {{if(starts_with(var('directory.path'),'/download'),'active')}}" href="/download/">
        <span>Download</span>
      </a>
    </li>
    <li class="nav-item">
      <a class="nav-link {{if(starts_with(var('directory.path'),'/blog'),'active')}}" href="/blog/">
        <span>Blog</span>
      </a>
    </li>
  </ul>
</div>
```

## Example HTML using templates
You can drop in and out of the templating syntax using the escaping sequence {{ }} 

/blog/index.html - Shows a list of recent blog posts
``` HTML
{#page.title:Blog Posts#}
<!DOCTYPE html>
<html lang="en">

<head>
  <!-- Global header includes (JS files, fonts etc) -->
  {{include('_header.html')}}
  <meta name="description" content="List of blog posts published by Piste">
  <title>Piste: {{var(page.title)}}</title>
</head>

<body class="d-flex flex-column min-vh-100">
  <!-- Static Top Menu -->
  {{include('_navbar.html')}}

  <!-- Content -->
  <div class="content container my-4">
    <h1>New Ski Blog Posts</h1>
    <div class="row">
      {{foreach(list_files('/blog/posts','*.md'),load_metadata(var('foreach.key'), include('_blogpost_stub.html')))}}
    </div>
  </div>

  <!-- Footer -->
  {{include('_footer.html')}}
</body>
</html>
```

Using load_metadata, the blog posts meta-data variables are loaded and ready for our stub to use and display:

/_partial/_blogpost_stub.html
```HTML
<div class="row g-0
  <div class="col-md-4">
    <img src="{{var('post.image')}}" class="blog-image img-fluid rounded-start" alt="Blog Post Image">
  </div>
  <div class="col-md-8">
    <div class="card-body">
      <h5 class="card-title">{{var('post.title')}}</h5>
      <p class="card-text">{{var('post.caption')}}</p>
      <div class="card-text" style="text-align: right;">
        <a href="{{get_url(var('foreach.key'))}}"
          class="btn btn-primary stretched-link float-right">Continue reading...</a>
        <br />
        <small class="text-muted">{{var('post.date')}}</small>
      </div>
    </div>
  </div>
</div>
```

## Example Markdown ##

Markdown files can be used to ease of content creation without the hassle of authoring individual HTML files. The markdown files get combined with a __template.html file to create a html sub directory.

Traditional templating syntax is not supported in markdown, but the metadata syntax is supported.

e.g mypost.md gets transformed into /mypost/index.html

/blog/posts/mypost1.md An example blog post with meta data prefixed (the metadata is used in the template to describe the page title, post date etc)
```MARKDOWN
{#post.title:Top 5 Ski Runs in Val d'Isère#}
{#post.date:16 September 2024#}
{#post.resort:Tignes Val d'Isere#}
{#post.caption:Did your favorite run make the list?#}
{#post.image:/assets/image/blog/topskirunvaldisere/header.png#}
# Top 5 Ski Runs in Val d'Isère

Heading off to Val d'Isère or Tignes this year? Don't miss these top runs. From beginner green to icy half pipes we think you'll find a favourite in our mix.

## 5. Glacier - Val d'Isere

![Glacier](/assets/image/blog/topskirunvaldisere/5.webp)

![float-end inline-image][Map of Glacier Piste](/assets/image/blog/topskirunvaldisere/5_map.png)

Val d'Isere has its own Glacier to rival Tignes's Grand Motte. A high-altitude gem that offers spectacular skiing on pristine snow, thanks to its location atop the Pissaillas Glacier. As a blue run, it provides a wide, gentle descent, making it ideal for intermediate skiers while offering stunning panoramic views of the surrounding peaks. The reliable snow conditions and breathtaking scenery make it a must-visit, particularly in the early season when lower-altitude runs may lack coverage. It’s a smooth, enjoyable ride with excellent snow, perfect for skiers looking to experience glacier skiing.
```

We obviously want to wrap our blog post into an actual HTML page, so we use the directory level __template.html to do this.

/blog/posts/__template.html an example template for all blog posts to compile into

```HTML
<!DOCTYPE html>
<html lang="en">
<head>
  {{include('_header.html')}}
  <meta name="description" content="{{var('post.caption')}}">
  <title>{{var('post.title')}}</title>
  <link href="/assets/css/piste_blog.css" rel="stylesheet">
</head>

<body class="d-flex flex-column min-vh-100">
  <!-- Static Top Menu -->
  {{include('_navbar.html')}}

  <!-- Content -->
  <div class="content container my-4">
    <!-- Breadcrumb -->
    <div class="row">
      <div class="col-md-8">
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="/">Home</a></li>
            <li class="breadcrumb-item" aria-current="page"><a href="/blog/">Blog</a></li>
            <li class="breadcrumb-item active" aria-current="page">{{var('post.title')}}</li>
          </ol>
        </nav>
      </div>
      <div class="col-md-4">
        <small class="float-end">Posted {{var('post.date')}}</small>
      </div>
    </div>
    <div class="row">
      <div class="col-md-8">
        <!-- Insert the markdown body here -->
        {{var('content')}}
      </div>
      <div class="col-md-4 d-none d-md-block">
        <h2>Related Posts</h2>
        {{
        foreach(list_files('/blog/posts','*.md').where(notequal(var('where.key'),var('input.fullname'))).take(5),
        load_metadata(var('foreach.key'),include('_blogpost_stub_horizontal.html')))}}
      </div>
    </div>
  </div>

  <!-- Footer -->
  {{include('_footer.html')}}
</body>
</html>
```

## Variable Scope

The processor pushes and pops variables as it goes though the directories and files, any variables you declare inside a HTML or md file only exists to sub files that are included in within it.

### Root Variables

This are available everywhere

| Variable |  Description   |
| ------------- | ------------- |
| root.fullpath | The full on-disk path the current working directory (Root) |
| root.url | The full url of the site root as set by -x |
| root.output |The full on-disk path of the output directory |


 ### Directory Variables 
 
 Each time a sub directory is entered the following variables are pushed onto the stack, subdirectories hide the parent directories variables.
 
| Variable |  Description   |
| ------------- | ------------- |
| directory.fullname | The full on-disk path the current input directory |
| directory.path | The relative "to root" path of the current directory |
| directory.name |The name of the current directory |

### HTML Input Variables

Each time a HTML file is processed the following variables are assigned
 
| Variable |  Description   |
| ------------- | ------------- |
| input.fullname | The full on-disk path the current input file |
| input.path | The relative "to root" path of the current file |
| input.name |The name of the current file |
| output.fullname | The full on-disk path the current output for the file |
| output.path | The relative on-disk "to root" path of the output for the file|
| output.url |The relative url of the output file generated |
| output.fullurl |The full url of the output file generated |


### Include Partial variables
Each time a Partial file is included the following variables are assigned
 
| Variable |  Description   |
| ------------- | ------------- |
| partial.fullname | The full on-disk path the current input file |
| partial.name |The name of the current partial file |



## Functions

### Numeric
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| Add | number1,number2 | number | Adds two numbers together and returns the result |
| Subtract | number1,number2 | number | Subtracts two numbers together and returns the result |
| Divide | number1,number2 | number | Divides two numbers together and returns the result |
| Multiply | number1,number2 | number | Multiplies two numbers together and returns the result |

### Variables
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| Assign | VariableName,value | - | Assigns VariableName to the specified value |
| Var | VariableName | value | Retrives the value stored under the variable name  |

Variables can also assigned using metadata flags in HTML and Markdown files, this is particually usefull when you want to get a summary information of a file without nesserially parsing the whole file.
```
{#VariableName:value#}
```

| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| load_metadata | input filename,body | result of body | Loads the metadata variables from a file into the stack then executes the body statement|

### Conditionals
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| equal | LHS,RHS | bool | Returns true if the result of LHS equals the result of RHS  |
| doesnotequal | LHS,RHS | bool | Returns true if the result of LHS does not equal the result of RHS  |
| if | bool,body if true, [body if false] | Selected Body | Returns the true body or false body depending on the conditional |0

### String opertaions
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| Concat | Str1,Str2,... | String | Concatinates all the paramaters into one string |
| Startswith | haystack,needle | Bool | Checks if the haystack starts with needle |


### Arrays
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| foreach | array,body,[foreachVariableName(def=foreach)] | array result of body | executes the body for each item in an array, the variables foreach.key and foreach.index are assigned before each body is executed|
| join | array,[seperator(def=,)] | concatinated result | Concatinates each item in the array together using the seperator charater DEF: , (CSV)|
| reverse | array | array | Reverses the array|
| shuffle | array | array | Randomises the array|
| skip | array,count | array | Skips the first X items in an array |
| take | array,count | array | Takes the first X items in an array |
| to_array | string | array | Splits a CSV string into an array |
| where | array, conditional bool, [whereVariableName(def=where) | array | Selects items out that match the conditional, a variable where.key is added to the stack before each conditional is executed, return true to include item |

### File Operations
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| get_url | input path | string | Finds the resultant URL of an input file once processed though the system i.e /blog/posts/markdown1.md -> /blog/posts/markdown1/index.html|
| include | input path | body | Parses then Prints the included file in place|
| list_files | input_path,[filter=(def:*)] | array | Lists all files within a directory with an optional filter|
| minify_url | url | string | Attempts to remove index.html from paths, i.e blog/posts/markdown1/index.html -> blog/posts/markdown1/|


