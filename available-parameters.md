---
layout: default
---
## Parameters ##

<table border="0" cellpadding="3" cellspacing="0" width="90%" id="tasksTable">
    <tr>
		<td>DestinationPath</td>
		<td><i>Required</i></td>
		<td>The end distanation to where the MSI will be published. By default the MSI file will be named as: <i>ApplicationName-yyyyMMddHHmmss</i>.</td>
	</tr>
    <tr>
		<td>ApplicationName</td>
		<td><i>Required</i></td>
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
		<td>A possible version number added to the MSI. Uses a <i>1.0.0.0</i> format.</td>
	</tr>
 	<tr>
		<td>FileName</td>
		<td><i>Optional</i></td>
		<td>If set if will be used as the MSI file name.</td>
	</tr>
 	<tr>
		<td>SourceLocation</td>
		<td><i>Optional</i></td>
		<td>If set will be part of the MSI property for source location and visible in the BizTalk Administration console.</td>
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

