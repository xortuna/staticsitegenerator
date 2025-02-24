# Static Site Generator for C# #

Dirt simple static site generator

Used to generate the https://pisteapp.com website

## Command Line

```
StaticSiteGenerator.exe -w -x "https://mysite.com"
```

## Arguments


## Variables


## Example

``` HTML
<!DOCTYPE html>
<html lang="en">

<head>
  {{include('_header.html')}}
  <meta name="description" content="List of blog posts published by Piste">
  <title>Piste: Blog</title>
  <style type="text/css">
    body {
      background-color: white;
      color: #333;
    }

    .content {
      font-family: 'Poppins';
    }

    .card img {
      object-fit: cover;
      height: 100%;
    }
  </style>
  <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300&&display=swap" rel="stylesheet" type='text/css'>
</head>

<body class="d-flex flex-column min-vh-100">
  <!-- Static Top Menu -->
  {{include('_navbar.html')}}

  <!-- SVG Wave Shape -->
  <div class="wave top">
    <svg viewBox="0 0 795 68" preserveAspectRatio="none">
      <path fill="#254f8f" fill-opacity="1" d="M 0,18 C 360,-20 500,100 795,0 530,0 260,0 0,0 Z" />
    </svg>
  </div>


  <!-- Content -->
  <div class="content container my-4">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="/">Home</a></li>
        <li class="breadcrumb-item active" aria-current="page">Blog</li>
      </ol>
    </nav>

    <h1>New Ski Blog Posts</h1>
    <p>Sometimes we write stuff about skiing and snowboarding, others we write about whatever takes our fancy! These are
      our latest and greatest articles for you to check out.</p>
    <div class="row">
      {{foreach(list_files('/blog/posts','*.md'),load_metadata(var('foreach.key'), include('_blogpost_stub.html')))}}
    </div>
  </div>

  <!-- Footer -->
  {{include('_footer.html')}}
</body>

</html>
```

# 
