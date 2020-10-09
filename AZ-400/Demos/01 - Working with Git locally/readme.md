# A simple guide to start with Git

## Working with a local repository
This command creates a new local repository with the name 'newRepo'
> Git init newRepo

We can now switch into the newly created directory
cd newRepo

Create a new file to demonstrate how Git works
`touch myFile.txt` (alternatively create via context menu)
Open the file in your preferred editor
> code myFile.txt

Now edit the file. Make a few changes. Write somethin interesting.

Create another file and repeat the process.
> touch myFile2.txt
> code myFile2.txt

Edit this file, too.

With Visual Studio Code open the folder `newRepo`.
Open `Git Bash`.
Display both side by side.

From Git Bash run
`git status` to see what git tells us about this repository.
You'll see somethin like this:
```bash
On branch master

No commits yet

Untracked files:
  (use "git add <file>..." to include in what will be committed)
        File 1.txt
        File 2.txt

nothing added to commit but untracked files present (use "git add" to track)

```

- We're working on the branch `master` (we'll get to branches later).
- There have been no commits yet.
- There are a bunch of untracked files
  -  Git sees those files but doesn't care about them yet.
- There is nothing added to commit

We want to add those files so that Git keeps track about their changes.
`git add . ` to add **all** files
`git add Filename.txt` to add *specific* files.

Run `git status` again.
```bash
On branch master

No commits yet

Changes to be committed:
  (use "git rm --cached <file>..." to unstage)
        new file:   File 1.txt
        new file:   Fiel 2.txt

```
- Still on branch `master`
- Still no commits
- Git recognizes our newly added files as `Changes to be commited`
  - Git keeps track of changes now but as we did not `commit` our changes yet they are *staged*. They are not added to the history yet.
  So for now, we only *see* if something has changed. We can not *revert* changes though.

**[!]** Before we commit our changes try changing one of the files and check what Git tells us with `git status`.

Now that our changes are staged we can commit them
> git commit -m "initial commit"

**[!]** Git might want you to tell it who you are. This is because Git keeps track of **who** made changes. To tell git who you are run:
`git config [--global] user.email your@email.net`
`git config [--global] user.name "Your Name"`
Omit `--global` to make these settings for this repository only.

If everything worked you'll see something like this:
```bash
[master (root-commit) b5632de] init
 2 files changed, 3 insertions(+)
 create mode 100644 File 1.txt
 create mode 100644 File 2.txt
```

Run `git status` again:
```bash
On branch master
nothing to commit, working tree clean
```
Our working tree is clean (no changes happened).

Now change one of the files and run these commands so we have a bunch of commits to check in our history:

> git add .
> git commit -m "Changes"

### Branching to keep the master
Making changes to the `master branch` isn't always a good idea. What if our changes break the application? We need a simple way to make changes without affecting the master branch but also without copying the entire project to another location.
That's where branches come handy!

We create a new `branch` with the name **newFeature**
> git branch newFeature

We switch (or checkout) the new branch.
(switch was implemented later, you can still use checkout)

> git checkout/switch newFeature

We create a new file.
> touch newFile3.txt

#####Now change some things in this and/or the other files.
Run
> git status
> git add .
> git commit -m "new feature added"

We made a few changes and commited those to our branch `newFeature`.
Run `git log` to see a history of what happened.
You should see something like this:
```bash
commit fef4ca88e3e23fd91577860a98f0a170a9d5ee43 (HEAD -> newFeature)
Author: Admin <Admin@blade.net>
Date:   Fri Oct 9 10:37:09 2020 +0200

    newFeature

commit b5632de13976b093aee3c7b39917c266d6ca7f04 (master)
Author: Admin <Admin@blade.net>
Date:   Fri Oct 9 10:20:20 2020 +0200

    init

```
There is our commit on the master branch (init) and our commit from our new branch (newFeature).

Let's switch back to our master branch to check it's history.

> git checkout/switch master
> git status
> git log

As you can see, there is no sign of our newFeature commit.
Also, you should notice that the file you created (while being on the newFeature branch) disappeared from the file system.

We are basically walking in two entirely different timelines.
One is the starting point (master) and the other is a literal branch of the master from a given point in time.

```javascript
[master]------[Commit1]
                  |
                  V
             [newFeature]------[Commit1]------[Commit2]
```

Not make changes to the files while you are on the master branch.
Then run:
> git add .
> git commit -m "changes on master"

Check the log.
Switch to the feature branch `git switch newFeature`.
Check the log.
Switch back to the master branch `git switch master`.

We now created something like this:
```javascript
[master]------[Commit1]--------[Commit2]------[Commit3]
                  |
                  V
             [newFeature]------[Commit1]------[Commit2]
```

Our master timeline did not stop while we were creating our feature.
While this is neat and convinient, at some point we'd like to join them back together. To create a master branch which contains our feature.
We'll have to `merge` our feature into the master branch.

Doing this is simple.
Switch to the master branch and run:
> git merge newFeature

```javascript
[master]------[Commit1]--------[Commit2]------[Commit3]------[...]
                  |                               ^
                  V                               |
             [newFeature]------[Commit1]------[Commit2]
```

However... There might be conflicts.
If File 1 was changed in the new feature branch **and** in the master branch Git will try to `merge` the changes.
This doesn't always work though. If for example the same line was changed in both branches Git can not know which change is correct.
You'll then have to
> resolve the conflicts

manually. 
So open the affected files and check for something like this:
```bash
Auto-merging File 3.txt
CONFLICT (content): Merge conflict in File 3.txt
Automatic merge failed; fix conflicts and then commit the result.
```
Opening `File 3.txt` shows this:
```bash
<<<<<<< HEAD
Some new file with changes
=======
Some text in feature branch
>>>>>>> newFeature
```

We have to decide what we want to do now. Keep the changes we did in HEAD (that's our pointer, we are merging `newFeature` into `master`) or take the changes we get from `newFeature`.
We can also `merge` them and keep both.
Whatever we do, at the end the lines `<<<<<<< HEAD`, `=======` and `>>>>>>> newFeature` need to be gone.
Once you removed the lines and made your preferred changes you can save the file, add it with `git add .` and `commit` the changes.