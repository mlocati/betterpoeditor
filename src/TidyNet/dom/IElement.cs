using System;
	
/*
* Copyright (c) 2000 World Wide Web Consortium,
* (Massachusetts Institute of Technology, Institut National de
* Recherche en Informatique et en Automatique, Keio University). All
* Rights Reserved. This program is distributed under the W3C's Software
* Intellectual Property License. This program is distributed in the
* hope that it will be useful, but WITHOUT ANY WARRANTY; without even
* the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
* PURPOSE.
* See W3C License http://www.w3.org/Consortium/Legal/ for more details.
*/
namespace TidyNet.Dom
{
	/// <summary> The <code>Element</code> interface represents an element in an HTML or XML 
	/// document. Elements may have attributes associated with them; since the 
	/// <code>Element</code> interface inherits from <code>Node</code>, the 
	/// generic <code>Node</code> interface attribute <code>attributes</code> may 
	/// be used to retrieve the set of all attributes for an element. There are 
	/// methods on the <code>Element</code> interface to retrieve either an 
	/// <code>Attr</code> object by name or an attribute value by name. In XML, 
	/// where an attribute value may contain entity references, an 
	/// <code>Attr</code> object should be retrieved to examine the possibly 
	/// fairly complex sub-tree representing the attribute value. On the other 
	/// hand, in HTML, where all attributes have simple string values, methods to 
	/// directly access an attribute value can safely be used as a convenience.In 
	/// DOM Level 2, the method <code>normalize</code> is inherited from the 
	/// <code>Node</code> interface where it was moved.
	/// <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113'>Document Object Model (DOM) Level 2 Core Specification</a>.
	/// </summary>
	internal interface IElement : INode
	{
		/// <summary> The name of the element. For example, in: 
		/// <pre> &lt;elementExample 
		/// id="demo"&gt; ... &lt;/elementExample&gt; , </pre>
		/// <code>tagName</code> has 
		/// the value <code>"elementExample"</code>. Note that this is 
		/// case-preserving in XML, as are all of the operations of the DOM. The 
		/// HTML DOM returns the <code>tagName</code> of an HTML element in the 
		/// canonical uppercase form, regardless of the case in the source HTML 
		/// document. 
		/// </summary>
		string TagName
		{
			get;
		}

		/// <summary> Retrieves an attribute value by name.</summary>
		/// <param name="nameThe">name of the attribute to retrieve.</param>
		/// <returns> The <code>Attr</code> value as a string, or the empty string 
		/// if that attribute does not have a specified or default value.
		/// </returns>
		string GetAttribute(string name);
		
		/// <summary> Adds a new attribute. If an attribute with that name is already present 
		/// in the element, its value is changed to be that of the value 
		/// parameter. This value is a simple string; it is not parsed as it is 
		/// being set. So any markup (such as syntax to be recognized as an 
		/// entity reference) is treated as literal text, and needs to be 
		/// appropriately escaped by the implementation when it is written out. 
		/// In order to assign an attribute value that contains entity 
		/// references, the user must create an <code>Attr</code> node plus any 
		/// <code>Text</code> and <code>EntityReference</code> nodes, build the 
		/// appropriate subtree, and use <code>setAttributeNode</code> to assign 
		/// it as the value of an attribute.
		/// <br>To set an attribute with a qualified name and namespace URI, use 
		/// the <code>setAttributeNS</code> method.
		/// </summary>
		/// <param name="nameThe">name of the attribute to create or alter.</param>
		/// <param name="valueValue">to set in string form.</param>
		/// <exception cref="DOMException">
		/// INVALID_CHARACTER_ERR: Raised if the specified name contains an 
		/// illegal character.
		/// <br />NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly.
		/// </exception>
		void SetAttribute(string name, string val);

		/// <summary> Removes an attribute by name. If the removed attribute is known to have 
		/// a default value, an attribute immediately appears containing the 
		/// default value as well as the corresponding namespace URI, local name, 
		/// and prefix when applicable.
		/// <br>To remove an attribute by local name and namespace URI, use the 
		/// <code>removeAttributeNS</code> method.
		/// </summary>
		/// <param name="nameThe">name of the attribute to remove.</param>
		/// <exception cref="DOMException">
		/// NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly.
		/// </exception>
		void RemoveAttribute(string name);

		/// <summary> Retrieves an attribute node by name.
		/// <br>To retrieve an attribute node by qualified name and namespace URI, 
		/// use the <code>getAttributeNodeNS</code> method.
		/// </summary>
		/// <param name="nameThe">name (<code>nodeName</code>) of the attribute to retrieve.</param>
		/// <returns> The <code>Attr</code> node with the specified name (<code>nodeName</code>)
		/// or <code>null</code> if there is no such  attribute.
		/// </returns>
		IAttr GetAttributeNode(string name);

		/// <summary> Adds a new attribute node. If an attribute with that name (
		/// <code>nodeName</code>) is already present in the element, it is 
		/// replaced by the new one.
		/// <br>To add a new attribute node with a qualified name and namespace 
		/// URI, use the <code>setAttributeNodeNS</code> method.
		/// </summary>
		/// <param name="newAttrThe"><code>Attr</code> node to add to the attribute list.
		/// </param>
		/// <returns> If the <code>newAttr</code> attribute replaces an existing 
		/// attribute, the replaced <code>Attr</code> node is returned, 
		/// otherwise <code>null</code> is returned.
		/// </returns>
		/// <exception cref="DOMException">
		/// WRONG_DOCUMENT_ERR: Raised if <code>newAttr</code> was created from a 
		/// different document than the one that created the element.
		/// <br />NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly.
		/// <br />INUSE_ATTRIBUTE_ERR: Raised if <code>newAttr</code> is already an 
		/// attribute of another <code>Element</code> object. The DOM user must 
		/// explicitly clone <code>Attr</code> nodes to re-use them in other 
		/// elements.
		/// </exception>
		IAttr SetAttributeNode(IAttr newAttr);

		/// <summary> Removes the specified attribute node. If the removed <code>Attr</code> 
		/// has a default value it is immediately replaced. The replacing 
		/// attribute has the same namespace URI and local name, as well as the 
		/// original prefix, when applicable.
		/// </summary>
		/// <param name="oldAttrThe"><code>Attr</code> node to remove from the attribute 
		/// list.
		/// </param>
		/// <returns>The <code>Attr</code> node that was removed.</returns>
		/// <exception cref="DOMException">
		/// NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly.
		/// <br />NOT_FOUND_ERR: Raised if <code>oldAttr</code> is not an attribute 
		/// of the element.
		/// </exception>
		IAttr RemoveAttributeNode(IAttr oldAttr);

		/// <summary> Returns a <code>NodeList</code> of all descendant <code>Elements</code> 
		/// with a given tag name, in the order in which they are encountered in 
		/// a preorder traversal of this <code>Element</code> tree.
		/// </summary>
		/// <param name="nameThe">name of the tag to match on. The special value "*" 
		/// matches all tags.
		/// </param>
		/// <returns> A list of matching <code>Element</code> nodes.</returns>
		INodeList GetElementsByTagName(string name);

		/// <summary> Retrieves an attribute value by local name and namespace URI. HTML-only 
		/// DOM implementations do not need to implement this method.
		/// </summary>
		/// <param name="namespaceURIThe">namespace URI of the attribute to retrieve.</param>
		/// <param name="localNameThe">local name of the attribute to retrieve.</param>
		/// <returns> The <code>Attr</code> value as a string, or the empty string 
		/// if that attribute does not have a specified or default value.
		/// </returns>
		string GetAttributeNS(string namespaceURI, string localName);

		/// <summary> Adds a new attribute. If an attribute with the same local name and 
		/// namespace URI is already present on the element, its prefix is 
		/// changed to be the prefix part of the <code>qualifiedName</code>, and 
		/// its value is changed to be the <code>value</code> parameter. This 
		/// value is a simple string; it is not parsed as it is being set. So any 
		/// markup (such as syntax to be recognized as an entity reference) is 
		/// treated as literal text, and needs to be appropriately escaped by the 
		/// implementation when it is written out. In order to assign an 
		/// attribute value that contains entity references, the user must create 
		/// an <code>Attr</code> node plus any <code>Text</code> and 
		/// <code>EntityReference</code> nodes, build the appropriate subtree, 
		/// and use <code>setAttributeNodeNS</code> or 
		/// <code>setAttributeNode</code> to assign it as the value of an 
		/// attribute.
		/// <br>HTML-only DOM implementations do not need to implement this method.
		/// </summary>
		/// <param name="namespaceURIThe">namespace URI of the attribute to create or 
		/// alter.</param>
		/// <param name="qualifiedNameThe">qualified name of the attribute to create or 
		/// alter.</param>
		/// <param name="valueThe">value to set in string form.</param>
		/// <exception cref="DOMException">
		/// INVALID_CHARACTER_ERR: Raised if the specified qualified name 
		/// contains an illegal character.
		/// <br />NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly.
		/// <br />NAMESPACE_ERR: Raised if the <code>qualifiedName</code> is 
		/// malformed, if the <code>qualifiedName</code> has a prefix and the 
		/// <code>namespaceURI</code> is <code>null</code>, if the 
		/// <code>qualifiedName</code> has a prefix that is "xml" and the 
		/// <code>namespaceURI</code> is different from "
		/// http://www.w3.org/XML/1998/namespace", or if the 
		/// <code>qualifiedName</code> is "xmlns" and the 
		/// <code>namespaceURI</code> is different from "
		/// http://www.w3.org/2000/xmlns/".
		/// </exception>
		void SetAttributeNS(string namespaceURI, string qualifiedName, string val);
		
		/// <summary> Removes an attribute by local name and namespace URI. If the removed 
		/// attribute has a default value it is immediately replaced. The 
		/// replacing attribute has the same namespace URI and local name, as 
		/// well as the original prefix.
		/// <br>HTML-only DOM implementations do not need to implement this method.
		/// </summary>
		/// <param name="namespaceURIThe">namespace URI of the attribute to remove.</param>
		/// <param name="localNameThe">local name of the attribute to remove.</param>
		/// <exception cref="DOMException">
		/// NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly.
		/// </exception>
		void RemoveAttributeNS(string namespaceURI, string localName);

		/// <summary> Retrieves an <code>Attr</code> node by local name and namespace URI. 
		/// HTML-only DOM implementations do not need to implement this method.
		/// </summary>
		/// <param name="namespaceURIThe">namespace URI of the attribute to retrieve.</param>
		/// <param name="localNameThe">local name of the attribute to retrieve.</param>
		/// <returns> The <code>Attr</code> node with the specified attribute local 
		/// name and namespace URI or <code>null</code> if there is no such 
		/// attribute.
		/// </returns>
		IAttr GetAttributeNodeNS(string namespaceURI, string localName);

		/// <summary> Adds a new attribute. If an attribute with that local name and that 
		/// namespace URI is already present in the element, it is replaced by 
		/// the new one.
		/// <br>HTML-only DOM implementations do not need to implement this method.
		/// </summary>
		/// <param name="newAttrThe"><code>Attr</code> node to add to the attribute list.</param>
		/// <returns> If the <code>newAttr</code> attribute replaces an existing 
		/// attribute with the same local name and namespace URI, the replaced 
		/// <code>Attr</code> node is returned, otherwise <code>null</code> is 
		/// returned.
		/// </returns>
		/// <exception cref="DOMException">
		/// WRONG_DOCUMENT_ERR: Raised if <code>newAttr</code> was created from a 
		/// different document than the one that created the element.
		/// <br />NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly.
		/// <br />INUSE_ATTRIBUTE_ERR: Raised if <code>newAttr</code> is already an 
		/// attribute of another <code>Element</code> object. The DOM user must 
		/// explicitly clone <code>Attr</code> nodes to re-use them in other 
		/// elements.
		/// </exception>
		IAttr SetAttributeNodeNS(IAttr newAttr);

		/// <summary> Returns a <code>NodeList</code> of all the descendant 
		/// <code>Elements</code> with a given local name and namespace URI in 
		/// the order in which they are encountered in a preorder traversal of 
		/// this <code>Element</code> tree.
		/// <br />HTML-only DOM implementations do not need to implement this method.
		/// </summary>
		/// <param name="namespaceURIThe">namespace URI of the elements to match on. The 
		/// special value "*" matches all namespaces.
		/// </param>
		/// <param name="localNameThe">local name of the elements to match on. The 
		/// special value "*" matches all local names.
		/// </param>
		/// <returns>A new <code>NodeList</code> object containing all the matched 
		/// <code>Elements</code>.
		/// </returns>
		INodeList GetElementsByTagNameNS(string namespaceURI, string localName);

		/// <summary> Returns <code>true</code> when an attribute with a given name is 
		/// specified on this element or has a default value, <code>false</code> 
		/// otherwise.
		/// </summary>
		/// <param name="nameThe">name of the attribute to look for.</param>
		/// <returns> <code>true</code> if an attribute with the given name is 
		/// specified on this element or has a default value, <code>false</code>
		/// otherwise.
		/// </returns>
		bool HasAttribute(string name);

		/// <summary> Returns <code>true</code> when an attribute with a given local name and 
		/// namespace URI is specified on this element or has a default value, 
		/// <code>false</code> otherwise. HTML-only DOM implementations do not 
		/// need to implement this method.
		/// </summary>
		/// <param name="namespaceURIThe">namespace URI of the attribute to look for.</param>
		/// <param name="localNameThe">local name of the attribute to look for.</param>
		/// <returns> <code>true</code> if an attribute with the given local name 
		/// and namespace URI is specified or has a default value on this 
		/// element, <code>false</code> otherwise.
		/// </returns>
		bool HasAttributeNS(string namespaceURI, string localName);
	}
}