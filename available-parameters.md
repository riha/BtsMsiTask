---
layout: default
title:	'Available parameters'
---
## Parameters ##

The tables show all available parameters. 

<table border="0" cellpadding="0" cellspacing="0" id="tasksTable">
    <tr>
		<td>DestinationPath</td>
		<td><em>Required</em></td>
		<td>The path to where the MSI will be published.<br />Example: <code>C:\Temp\BtsSample</code>.</td>
	</tr>
    <tr>
		<td>ApplicationName</td>
		<td><em>Required</em></td>
		<td>The BizTalk Application that be created/updated by the MSI.</td>
	</tr>
    <tr>
		<td>ApplicationDescription</td>
		<td>Optional</td>
		<td>The BizTalk Description that will be added/updated to the BizTalk Application.</td>
	</tr>
    <tr>
		<td>Version</td>
		<td>Optional</td>
		<td>A possible version number added to the MSI. Uses a <code>1.0.0.0</code> format.</td>
	</tr>
 	<tr>
		<td>FileName</td>
		<td><i>Optional</i></td>
		<td>If set if will be used as the MSI file name. Defaults to <code>ApplicationName-yyyyMMddHHmmss</code><br />Example: <code>Build 23456_2.msi</code></td>
	</tr>
 	<tr>
		<td>SourceLocation</td>
		<td><i>Optional</i></td>
		<td>If set will be part of the MSI property for source location and visible in the BizTalk Administration console.<br /> Example: <code>\\acme.com\drops$\Build 23456_2</code></td>
	</tr>
	<tr>
		<td>BtsAssemblies (ItemGroup)</td>
		<td><i>Optional</i></td>
		<td>A list of <i>BizTalk</i> resources that should be added to the MSI.</td>
	</tr>
	<tr>
		<td>Resources (ItemGroup)</td>
		<td><i>Optional</i></td>
		<td>A list of <i>Non BizTalk</i> resouses that should be added to the MSI.</td>
	</tr>
    <tr>
		<td>ReferenceApplications (ItemGroup)</td>
		<td>Optional</td>
		<td>List of BizTalk application that should be refernces as the MIS is imported. By default the <i>System.BizTalk</i> is added.</td>
	</tr>
</table>

